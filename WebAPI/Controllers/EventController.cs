using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO.Output;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/event")]
    public class EventController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public EventController(ScoreHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDetailsDTO>> GetEventDetails(long id, CancellationToken ct)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Venue)
                .Include(m => m.League)
                .Include(m => m.Statistics)
                .Include(m => m.Lineups).ThenInclude(l => l.Player)
                .Include(m => m.TvBroadcasts).ThenInclude(t => t.Channel)
                .Include(m => m.Timeline).ThenInclude(t => t.Player)
                .Include(m => m.Timeline).ThenInclude(t => t.Assistant)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, ct);

            if (match is null)
                return NotFound();

            var result = new EventDetailsDTO
            {
                EventName = match.Name,
                EventDate = match.EventDate,
                HomeTeamId = match.HomeTeamId,
                AwayTeamId = match.AwayTeamId,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                HomeTeamBadge = match.HomeTeam.LogoUrl,
                AwayTeamBadge = match.AwayTeam.LogoUrl,
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                Progress = match.Progress,
                Venue = match.Venue?.Name ?? string.Empty,
                Status = MatchStateCalculator.ComputeState(match, DateTime.UtcNow),
                DetailedStatus = match.Status,
                Season = match.Season,
                LeagueName = match.League.Name,
                Round = string.IsNullOrWhiteSpace(match.Round) ? null : match.Round,
                Players = match.Lineups.Select(l => new EventLineupDTO
                {
                    PlayerId = l.PlayerId,
                    TeamId = l.TeamId,
                    FirstName = l.Player.FirstName,
                    LastName = l.Player.LastName,
                    Position = l.Position,
                    PositionShort = l.PositionShort,
                    Formation = l.Formation,
                    HomeGame = l.HomeGame,
                    IsSubtitute = l.IsSubstitute,
                    SquadNumber = l.SquadNumber ?? 0,
                    ImageUrl = l.Player.ImageUrl
                }).ToList(),
                Statistics = match.Statistics.Select(s => new EventStatisticsDTO
                {
                    Stat = s.Stat,
                    HomeScore = (float)s.HomeScore,
                    AwayScore = (float)s.AwayScore
                }).ToList(),
                Broadcasts = match.TvBroadcasts
                    .Where(t => !string.IsNullOrWhiteSpace(t.Channel.Name) || !string.IsNullOrWhiteSpace(t.Channel.LogoUrl))
                    .OrderBy(t => t.Channel.Country)
                    .ThenBy(t => t.Channel.Name)
                    .Select(t => new EventBroadcastDTO
                    {
                        ChannelName = t.Channel.Name,
                        LogoUrl = t.Channel.LogoUrl,
                        Country = t.Channel.Country
                    }).ToList(),
                Timeline = match.Timeline
                    .Where(t => (string.Equals(t.Type, "Goal", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Type, "Card", StringComparison.OrdinalIgnoreCase))
                        && (t.Detail == null || !t.Detail.Contains("Missed", StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(t => t.Minute ?? int.MaxValue)
                    .ThenBy(t => t.Id)
                    .Select(t => new EventTimelineDTO
                    {
                        Minute = t.Minute,
                        Type = t.Type,
                        Detail = t.Detail,
                        Home = t.Home,
                        Comment = t.Comment,
                        PlayerName = t.Player is null ? null : $"{t.Player.FirstName} {t.Player.LastName}".Trim(),
                        AssistName = t.Assistant is null ? null : $"{t.Assistant.FirstName} {t.Assistant.LastName}".Trim()
                    }).ToList(),
                FullTimeline = match.Timeline
                    .OrderBy(t => t.Minute ?? int.MaxValue)
                    .ThenBy(t => t.Id)
                    .Select(t => new EventTimelineDTO
                    {
                        Minute = t.Minute,
                        Type = t.Type,
                        Detail = t.Detail,
                        Home = t.Home,
                        Comment = t.Comment,
                        PlayerName = t.Player is null ? null : $"{t.Player.FirstName} {t.Player.LastName}".Trim(),
                        AssistName = t.Assistant is null ? null : $"{t.Assistant.FirstName} {t.Assistant.LastName}".Trim()
                    }).ToList()
            };

            return Ok(result);
        }
    }
}
