using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO.Output;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/league")]
    public class LeagueController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public LeagueController(ScoreHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeagueDetailsDTO>> GetLeagueDetails(long id, CancellationToken ct)
        {
            var league = await _context.Leagues.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, ct);

            if (league is null)
                return NotFound();

            var teams = await _context.Teams
                .Where(t => t.LeagueId == id)
                .OrderBy(t => t.Name)
                .AsNoTracking()
                .ToListAsync(ct);


            var standings = await _context.Standings
                .Include(s => s.Team)
                .Where(s => s.LeagueId == id && s.Season == league.CurrentSeason)
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.GoalDifference)
                .ThenBy(s => s.Rank)
                .AsNoTracking()
                .ToListAsync(ct);

            var result = new LeagueDetailsDTO
            {
                Name = league.Name,
                LogoUrl = league.LogoUrl,
                Country = league.Country,
                Teams = teams.Select(t => new LeagueTeamDTO
                {
                    TeamId = t.Id,
                    LogoUrl = t.LogoUrl,
                    TeamName = t.Name,
                    ShortName = t.ShortName
                }).ToList(),
                Standings = standings.Select(s => new LeagueStandingDTO
                {
                    Rank = s.Rank,
                    TeamId = s.TeamId,
                    TeamName = s.Team.Name,
                    TeamShortName = s.Team.ShortName,
                    TeamLogoUrl = s.Team.LogoUrl,
                    MatchesPlayed = s.MatchesPlayed,
                    MatchesWon = s.MatchesWon,
                    MatchesDraw = s.MatchesDraw,
                    MatchesLost = s.MatchesLost,
                    GoalsFor = s.GoalsFor,
                    GoalsAgainst = s.GoalsAgainst,
                    GoalDifference = s.GoalDifference,
                    Points = s.Points,
                    Description = s.Description,
                    Form = s.Form
                }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<LeagueSearchResultDTO>>> SearchLeagues([FromQuery] string query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<LeagueSearchResultDTO>());

            var leagues = await _context.Leagues
                .Where(l => l.Name.Contains(query))
                .OrderBy(l => l.Name)
                .Take(50)
                .Select(l => new LeagueSearchResultDTO
                {
                    Id = l.Id,
                    Name = l.Name,
                    ImageUrl = l.LogoUrl
                })
                .ToListAsync(ct);

            return Ok(leagues);
        }
    }
}
