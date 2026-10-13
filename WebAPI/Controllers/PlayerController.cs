using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO.Output;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/player")]
    public class PlayerController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public PlayerController(ScoreHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDTO>> GetPlayerDetails(long id, CancellationToken ct)
        {
            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.SecondaryTeam)
                .Include(p => p.Statistics).ThenInclude(s => s.Team)
                .Include(p => p.Statistics).ThenInclude(s => s.League)
                .Include(p => p.Honours).ThenInclude(h => h.Team)
                .Include(p => p.Honours).ThenInclude(h => h.League)
                .Include(p => p.Milestones).ThenInclude(m => m.Team)
                .Include(p => p.FormerTeams).ThenInclude(ft => ft.FormerTeam)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            if (player is null)
                return NotFound();

            var result = new PlayerDTO
            {
                FirstName = player.FirstName,
                LastName = player.LastName,
                ImageUrl = player.ImageUrl,
                BirthDate = player.BirthDate.ToDateTime(TimeOnly.MinValue),
                BirthLocation = player.BirthLocation,
                Team = player.Team?.Name,
                SecondaryTeam = player.SecondaryTeam?.Name,
                Position = player.Position,
                Description = player.Description,
                Status = player.Status,
                Weight = player.Weight,
                Height = player.Height,
                Worth = player.Worth,
                FacebookLink = player.FacebookLink,
                InstagramLink = player.InstagramLink,
                TwitterLink = player.TwitterLink,
                YoutubeLink = player.YouTubeLink,
                Statistics = player.Statistics.Select(s => new PlayerStatistics
                {
                    Team = s.Team?.Name,
                    League = s.League?.Name,
                    Statistic = s.Statistic,
                    Value = s.Value,
                    Season = s.Season
                }).ToList(),
                Honours = player.Honours.Select(h => new PlayerHonours
                {
                    Team = h.Team?.Name,
                    League = h.League?.Name,
                    HonourName = h.HonourName,
                    HonourTrophyUrl = h.HonourTrophyUrl,
                    Season = h.Season
                }).ToList(),
                Milestones = player.Milestones.Select(m => new PlayerMilestones
                {
                    Team = m.Team?.Name,
                    MilestoneName = m.MilestoneName,
                    MilestoneUrl = m.MilestoneLogoUrl,
                    DateAchieved = m.DateAchieved ?? default
                }).ToList(),
                FormerTeams = player.FormerTeams
                    .OrderBy(ft => int.TryParse(ft.Joined, out var joinedYear) ? joinedYear : int.MaxValue)
                    .Select(ft => new PlayerFormerTeams
                {
                    Team = ft.FormerTeam?.Name,
                    TeamLogoUrl = ft.FormerTeam?.LogoUrl,
                    MoveType = ft.MoveType,
                    Joined = ft.Joined,
                    Departed = ft.Departed,
                    Appearences = ft.Appearances?.ToString(),
                    Goals = ft.Goals?.ToString()
                }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<PlayerSearchResultDTO>>> SearchPlayers([FromQuery] string query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<PlayerSearchResultDTO>());

            var players = await _context.Players
                .Where(p => p.FirstName.Contains(query)
                         || p.LastName.Contains(query)
                         || (p.FirstName + " " + p.LastName).Contains(query))
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .Take(50)
                .Select(p => new PlayerSearchResultDTO
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    ImageUrl = p.ImageUrl,
                    SportName = p.Team != null ? p.Team.League.Sport.Name : null
                })
                .ToListAsync(ct);

            return Ok(players);
        }
    }
}
