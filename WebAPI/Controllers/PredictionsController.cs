using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.DTO.Input;
using WebAPI.DTO.Output;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/predictions")]

    [Authorize]
    public class PredictionsController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public PredictionsController(ScoreHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult<List<MyPredictionDTO>>> GetMyPredictions(CancellationToken ct)
        {
            var userId = GetUserId();

            if (userId is null)
                return Unauthorized();

            var nowUtc = DateTime.UtcNow;

            var predictions = await _context.Predictions
                .AsNoTracking()
                .Where(p => p.UserId == userId.Value)
                .Select(p => new
                {
                    p.MatchId,
                    p.PredictedHomeScore,
                    p.PredictedAwayScore,
                    p.Match.EventDate,
                    p.Match.Status,
                    p.Match.HomeTeamScore,
                    p.Match.AwayTeamScore,
                    HomeTeam = p.Match.HomeTeam.Name,
                    AwayTeam = p.Match.AwayTeam.Name
                })
                .ToListAsync(ct);

            var result = predictions
                .OrderBy(p => p.EventDate)
                .Select(p => new MyPredictionDTO
                {
                    EventId = p.MatchId,
                    EventDate = p.EventDate,
                    HomeTeam = p.HomeTeam,
                    AwayTeam = p.AwayTeam,
                    HomeTeamScore = p.HomeTeamScore,
                    AwayTeamScore = p.AwayTeamScore,
                    Status = MatchStateCalculator.ComputeState(p.EventDate, p.Status, nowUtc),
                    PredictedHomeScore = p.PredictedHomeScore,
                    PredictedAwayScore = p.PredictedAwayScore
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<PredictionDTO>> GetMyPrediction(long matchId, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var prediction = await _context.Predictions
                .AsNoTracking()
                .Where(p => p.UserId == userId.Value && p.MatchId == matchId)
                .Select(p => new PredictionDTO
                {
                    PredictedHomeScore = p.PredictedHomeScore,
                    PredictedAwayScore = p.PredictedAwayScore
                })
                .FirstOrDefaultAsync(ct);

            if (prediction is null)
                return NotFound();

            return Ok(prediction);
        }


        [HttpPut("match/{matchId}")]
        public async Task<IActionResult> SavePrediction(long matchId, [FromBody] SavePredictionDTO request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var match = await _context.Matches
                .AsNoTracking()
                .Where(m => m.Id == matchId)
                .Select(m => new { m.EventDate, m.Status })
                .FirstOrDefaultAsync(ct);

            if (match is null)
                return NotFound();

            if (MatchStateCalculator.ComputeState(match.EventDate, match.Status, DateTime.UtcNow) != "Upcoming")
                return BadRequest(new ErrorResponseDTO { Errors = { "Predictions are closed - the match has already started." } });

            var prediction = await _context.Predictions.FirstOrDefaultAsync(p => p.UserId == userId.Value && p.MatchId == matchId, ct);
            if (prediction is null)
            {
                _context.Predictions.Add(new Prediction
                {
                    UserId = userId.Value,
                    MatchId = matchId,
                    PredictedHomeScore = request.PredictedHomeScore,
                    PredictedAwayScore = request.PredictedAwayScore
                });
            }
            else
            {
                prediction.PredictedHomeScore = request.PredictedHomeScore;
                prediction.PredictedAwayScore = request.PredictedAwayScore;
            }

            await _context.SaveChangesAsync(ct);
            return Ok();
        }

        private long? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var id) ? id : null;
        }
    }
}
