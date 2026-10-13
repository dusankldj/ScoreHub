using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO.Input;
using WebAPI.DTO.Output;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/matches")]
    public class MatchesController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public MatchesController(ScoreHubDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MatchesDTO>>> GetMatches([FromQuery] MatchesFiltersDTO filters, CancellationToken ct)
        {
            var nowUtc = DateTime.UtcNow;


            var query = _context.Matches
                .Include(m => m.League).ThenInclude(l => l.Sport)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.EventDate >= filters.From && m.EventDate < filters.To);

            if (filters.SportId.HasValue)
                query = query.Where(m => m.League.SportId == filters.SportId.Value);

            var matches = await query
                .OrderBy(m => m.League.Sport.Name)
                .ThenBy(m => m.League.Name)
                .ThenBy(m => m.EventDate)
                .AsNoTracking()
                .ToListAsync(ct);

 
            var withState = matches.Select(m => new { Match = m, State = MatchStateCalculator.ComputeState(m, nowUtc) });

            if (!string.IsNullOrEmpty(filters.Status))
                withState = withState.Where(x => x.State == filters.Status);

            var result = withState
                .GroupBy(x => new { x.Match.League.SportId, SportName = x.Match.League.Sport.Name })
                .Select(sportGroup => new MatchesDTO
                {
                    SportId = sportGroup.Key.SportId,
                    SportName = sportGroup.Key.SportName,
                    Leagues = sportGroup
                        .GroupBy(x => new { x.Match.LeagueId, LeagueName = x.Match.League.Name, LeagueLogoUrl = x.Match.League.LogoUrl })
                        .Select(leagueGroup => new LeaguesDTO
                        {
                            LeagueId = leagueGroup.Key.LeagueId,
                            LeagueName = leagueGroup.Key.LeagueName,
                            LeagueLogoUrl = leagueGroup.Key.LeagueLogoUrl,
                            Events = leagueGroup.Select(x => new EventsDTO
                            {
                                EventId = x.Match.Id,
                                EventDate = x.Match.EventDate,
                                HomeTeam = x.Match.HomeTeam.Name,
                                AwayTeam = x.Match.AwayTeam.Name,
                                HomeTeamLogoUrl = x.Match.HomeTeam.LogoUrl,
                                AwayTeamLogoUrl = x.Match.AwayTeam.LogoUrl,
                                HomeTeamScore = x.Match.HomeTeamScore,
                                AwayTeamScore = x.Match.AwayTeamScore,
                                Status = x.State,
                                DetailedStatus = x.Match.Status,
                                Progress = x.Match.Progress
                            }).ToList()
                        }).ToList()
                }).ToList();

            return Ok(result);
        }
    }
}
