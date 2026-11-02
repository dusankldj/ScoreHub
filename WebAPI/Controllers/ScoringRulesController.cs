using DataAccess;
using Domain.Predictions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO.Output;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/scoring-rules")]
    public class ScoringRulesController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public ScoringRulesController(ScoreHubDbContext context)
        {
            _context = context;
        }

        // Pravila bodovanja svih sportova (javno - nema licnih podataka) sa primerima za stranicu "How points are scored".
        [HttpGet]
        public async Task<ActionResult<List<SportScoringDTO>>> GetRules(CancellationToken ct)
        {
            var rows = await _context.SportScoringRules
                .AsNoTracking()
                .OrderBy(r => r.Sport.Name)
                .Select(r => new
                {
                    r.SportId,
                    SportName = r.Sport.Name,
                    r.ExactPoints,
                    r.ClosePoints,
                    r.WinnerPoints,
                    r.CloseMarginTolerance,
                    r.MaxScore
                })
                .ToListAsync(ct);

            var result = rows.Select(r =>
            {
                var rule = new ScoringRule(r.ExactPoints, r.ClosePoints, r.WinnerPoints, r.CloseMarginTolerance, r.MaxScore);
                var (predHome, predAway, results) = ScoringExamples.For(r.SportName);

                return new SportScoringDTO
                {
                    SportId = r.SportId,
                    SportName = r.SportName,
                    ExactPoints = r.ExactPoints,
                    ClosePoints = r.ClosePoints,
                    WinnerPoints = r.WinnerPoints,
                    CloseDescription = rule.CloseDescription,
                    MaxScore = r.MaxScore,
                    ExamplePrediction = $"{predHome}-{predAway}",
                    Examples = results.Select(x =>
                    {
                        var outcome = PredictionScoring.Evaluate("FT", x.Home, x.Away, predHome, predAway, rule);
                        return new ScoringExampleDTO
                        {
                            Result = $"{x.Home}-{x.Away}",
                            Outcome = outcome.ToString(),
                            Points = PredictionScoring.Points(outcome, rule)
                        };
                    }).ToList()
                };
            }).ToList();

            return Ok(result);
        }
    }
}
