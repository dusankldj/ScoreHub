using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.DTO.Output;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/team")]
    public class TeamController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public TeamController(ScoreHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDTO>> GetTeamDetails(long id, CancellationToken ct)
        {
            var team = await _context.Teams
                .Include(t => t.Stadium)
                .Include(t => t.League).ThenInclude(l => l.Sport)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id, ct);

            if (team is null)
                return NotFound();

            var nowUtc = DateTime.UtcNow;


            var currentSeason = await _context.Matches
                .Where(m => m.HomeTeamId == id || m.AwayTeamId == id)
                .OrderByDescending(m => m.EventDate)
                .Select(m => m.Season)
                .FirstOrDefaultAsync(ct);

            var allTeamMatches = await _context.Matches
                .Include(m => m.League)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeamId == id || m.AwayTeamId == id)
                .OrderBy(m => m.EventDate)
                .AsNoTracking()
                .ToListAsync(ct);

 
            const int windowSize = 20;

            var withState = allTeamMatches
                .Select(m => new { Match = m, State = MatchStateCalculator.ComputeState(m, nowUtc) })
                .ToList();

            var lastPlayedIndex = withState.FindLastIndex(x => x.State != "Upcoming");

            var matches = withState
                .Take(lastPlayedIndex + 1).TakeLast(windowSize)
                .Concat(withState.Skip(lastPlayedIndex + 1).Take(windowSize))
                .Select(x => x.Match)
                .ToList();

            // Igrac moze biti vezan za OVAJ tim kao primarni (klub) ili sekundarni
            // (reprezentacija, ili obrnuto - API ne garantuje redosled) - prikazujemo ga u
            // sastavu bilo koje od te dve uloge.
            var squad = await _context.Players
                .Where(p => p.TeamId == id || p.SecondaryTeamId == id)
                .AsNoTracking()
                .ToListAsync(ct);

 
            var allEquipment = await _context.TeamEquipment
                .Where(e => e.TeamId == id)
                .AsNoTracking()
                .ToListAsync(ct);

            // Prioritet: TACNO tekuca sezona (currentSeason, isti izvor kao Standings rezerva -
            // poslednji mec tima). Ako dresovi za nju JOS NISU sinhronizovani (nova sezona zna
            // da krene pre nego sto SportsDB sync stigne do novih dresova), padamo na najskoriju
            // sezonu koja STVARNO postoji za ovaj tim - nikad prazna sekcija bez razloga.
            var equipment = allEquipment.Where(e => e.Season == currentSeason).ToList();

            if (equipment.Count == 0)
            {
                var latestEquipmentSeason = allEquipment
                    .Where(e => !string.IsNullOrWhiteSpace(e.Season))
                    .OrderByDescending(e => ExtractLeadingYear(e.Season))
                    .Select(e => e.Season)
                    .FirstOrDefault();

                equipment = allEquipment
                    .Where(e => e.Season == latestEquipmentSeason)
                    .ToList();
            }

 
            equipment = equipment
                .GroupBy(e => e.Type)
                .Select(g => g.Count() > 1 ? g.ElementAt(1) : g.First())
                .ToList();

            var result = new TeamDTO
            {
                Name = team.Name,
                ShortName = team.ShortName,
                ImageUrl = team.LogoUrl,
                Stadium = team.Stadium?.Name,
                Country = team.Country,
                SportName = team.League?.Sport?.Name,
                Squad = squad.Select(p => new Squad
                {
                    PlayerId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    ImageUrl = p.ImageUrl,
                    BirthDate = p.BirthDate.ToDateTime(TimeOnly.MinValue),
                    Position = p.Position
                }).ToList(),
                Equipment = equipment.Select(e => new DTO.Output.TeamEquipment
                {
                    ImageUrl = e.Equipment,
                    Type = e.Type,
                    Username = e.Username
                }).ToList(),
                Matches = matches.Select(m => new TeamMatches
                {
                    EventId = m.Id,
                    EventDate = m.EventDate,
                    League = m.League.Name,
                    Season = m.Season,
                    Round = m.Round,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamName = m.AwayTeam.Name,
                    HomeTeamShortName = m.HomeTeam.ShortName,
                    AwayTeamShortName = m.AwayTeam.ShortName,
                    HomeTeamLogo = m.HomeTeam.LogoUrl,
                    AwayTeamLogo = m.AwayTeam.LogoUrl,
                    HomeTeamScore = m.HomeTeamScore?.ToString() ?? string.Empty,
                    AwayTeamScore = m.AwayTeamScore?.ToString() ?? string.Empty,
                    Status = MatchStateCalculator.ComputeState(m, nowUtc)
                }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<TeamSearchResultDTO>>> SearchTeams([FromQuery] string query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<TeamSearchResultDTO>());


            var teams = await _context.Teams
                .Where(t => t.Name.Contains(query))
                .OrderBy(t => t.Name)
                .Take(50)
                .Select(t => new TeamSearchResultDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    ImageUrl = t.LogoUrl,
                    SportName = t.League != null ? t.League.Sport.Name : null
                })
                .ToListAsync(ct);

            return Ok(teams);
        }

        // "2025-2026" -> 2025, "2020" -> 2020, nepoznat/prazan format -> 0 (ide na dno sortiranja).
        private static int ExtractLeadingYear(string? season)
        {
            if (string.IsNullOrWhiteSpace(season))
                return 0;

            var digits = new string(season.TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(digits, out var year) ? year : 0;
        }

        [HttpGet("{id}/favorite")]
        [Authorize]
        public async Task<ActionResult<bool>> IsTeamFavorited(long id, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var isFavorited = await _context.FavoriteTeams.AnyAsync(f => f.UserId == userId.Value && f.TeamId == id, ct);
            return Ok(isFavorited);
        }

        [HttpPost("{id}/favorite")]
        [Authorize]
        public async Task<IActionResult> AddFavoriteTeam(long id, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var alreadyFavorited = await _context.FavoriteTeams.AnyAsync(f => f.UserId == userId.Value && f.TeamId == id, ct);
            if (!alreadyFavorited)
            {
                _context.FavoriteTeams.Add(new FavoriteTeam { UserId = userId.Value, TeamId = id, Notifications = true });
                await _context.SaveChangesAsync(ct);
            }

            return Ok();
        }

        [HttpDelete("{id}/favorite")]
        [Authorize]
        public async Task<IActionResult> RemoveFavoriteTeam(long id, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var favorite = await _context.FavoriteTeams.FirstOrDefaultAsync(f => f.UserId == userId.Value && f.TeamId == id, ct);
            if (favorite is not null)
            {
                _context.FavoriteTeams.Remove(favorite);
                await _context.SaveChangesAsync(ct);
            }

            return Ok();
        }

        private long? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var id) ? id : null;
        }
    }
}
