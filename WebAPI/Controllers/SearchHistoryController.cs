using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.DTO.Input;
using WebAPI.DTO.Output;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/users/me/searches")]
    [Authorize]
    public class SearchHistoryController : ControllerBase
    {
        // Koliko poslednjih pretrazivanja cuvamo/prikazujemo po korisniku - visak se brise pri upisu.
        private const int MaxRecentSearches = 10;

        private readonly ScoreHubDbContext _context;

        public SearchHistoryController(ScoreHubDbContext context)
        {
            _context = context;
        }

        // Od poslednjeg ka prvom, vec "popunjeno" (ime/slika/sport) da klijent ne mora nista dodatno da trazi.
        [HttpGet]
        public async Task<ActionResult<List<RecentSearchDTO>>> GetRecentSearches(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var searches = await _context.UserSearches
                .AsNoTracking()
                .Where(s => s.UserId == userId.Value)
                .OrderByDescending(s => s.SearchedAt)
                .Take(MaxRecentSearches)
                .ToListAsync(ct);

            var playerIds = searches.Where(s => s.Kind == UserSearchKind.Player).Select(s => s.EntityId).ToList();
            var teamIds = searches.Where(s => s.Kind == UserSearchKind.Team).Select(s => s.EntityId).ToList();
            var leagueIds = searches.Where(s => s.Kind == UserSearchKind.League).Select(s => s.EntityId).ToList();

            var players = await _context.Players
                .AsNoTracking()
                .Where(p => playerIds.Contains(p.Id))
                .Select(p => new { p.Id, p.FirstName, p.LastName, p.ImageUrl, SportName = p.Team != null ? p.Team.League.Sport.Name : null })
                .ToDictionaryAsync(p => p.Id, ct);

            var teams = await _context.Teams
                .AsNoTracking()
                .Where(t => teamIds.Contains(t.Id))
                .Select(t => new { t.Id, t.Name, t.LogoUrl, SportName = t.League != null ? t.League.Sport.Name : null })
                .ToDictionaryAsync(t => t.Id, ct);

            var leagues = await _context.Leagues
                .AsNoTracking()
                .Where(l => leagueIds.Contains(l.Id))
                .Select(l => new { l.Id, l.Name, l.LogoUrl, SportName = l.Sport.Name })
                .ToDictionaryAsync(l => l.Id, ct);

            var result = new List<RecentSearchDTO>();

            foreach (var search in searches)
            {
                // Entitet koji vise ne postoji se tiho preskace (vidi UserSearch - "siroceta").
                switch (search.Kind)
                {
                    case UserSearchKind.Player when players.TryGetValue(search.EntityId, out var p):
                        result.Add(new RecentSearchDTO
                        {
                            Id = search.Id,
                            Kind = nameof(UserSearchKind.Player),
                            EntityId = p.Id,
                            DisplayName = $"{p.FirstName} {p.LastName}".Trim(),
                            ImageUrl = p.ImageUrl,
                            SportName = p.SportName
                        });
                        break;

                    case UserSearchKind.Team when teams.TryGetValue(search.EntityId, out var t):
                        result.Add(new RecentSearchDTO
                        {
                            Id = search.Id,
                            Kind = nameof(UserSearchKind.Team),
                            EntityId = t.Id,
                            DisplayName = t.Name,
                            ImageUrl = t.LogoUrl,
                            SportName = t.SportName
                        });
                        break;

                    case UserSearchKind.League when leagues.TryGetValue(search.EntityId, out var l):
                        result.Add(new RecentSearchDTO
                        {
                            Id = search.Id,
                            Kind = nameof(UserSearchKind.League),
                            EntityId = l.Id,
                            DisplayName = l.Name,
                            ImageUrl = l.LogoUrl,
                            SportName = l.SportName
                        });
                        break;
                }
            }

            return Ok(result);
        }

        // Poziva se kad korisnik IZABERE rezultat pretrage. Isti entitet ponovo = samo se SearchedAt pomera
        // na sada (unique indeks UserId+Kind+EntityId), pa se stavka vraca na vrh umesto da se duplira.
        [HttpPost]
        public async Task<IActionResult> RecordSearch([FromBody] RecordSearchDTO request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            if (request is null || !Enum.TryParse<UserSearchKind>(request.Kind, ignoreCase: true, out var kind) || !Enum.IsDefined(kind))
                return BadRequest();

            var exists = kind switch
            {
                UserSearchKind.Player => await _context.Players.AnyAsync(p => p.Id == request.EntityId, ct),
                UserSearchKind.Team => await _context.Teams.AnyAsync(t => t.Id == request.EntityId, ct),
                UserSearchKind.League => await _context.Leagues.AnyAsync(l => l.Id == request.EntityId, ct),
                _ => false
            };

            if (!exists)
                return NotFound();

            var now = DateTime.UtcNow;

            var existing = await _context.UserSearches.FirstOrDefaultAsync(
                s => s.UserId == userId.Value && s.Kind == kind && s.EntityId == request.EntityId, ct);

            if (existing is null)
            {
                _context.UserSearches.Add(new UserSearch
                {
                    UserId = userId.Value,
                    Kind = kind,
                    EntityId = request.EntityId,
                    SearchedAt = now
                });
            }
            else
            {
                existing.SearchedAt = now;
            }

            await _context.SaveChangesAsync(ct);

            // Zadrzi samo najnovijih MaxRecentSearches - sve starije se brise.
            var staleIds = await _context.UserSearches
                .Where(s => s.UserId == userId.Value)
                .OrderByDescending(s => s.SearchedAt)
                .Skip(MaxRecentSearches)
                .Select(s => s.Id)
                .ToListAsync(ct);

            if (staleIds.Count > 0)
                await _context.UserSearches.Where(s => staleIds.Contains(s.Id)).ExecuteDeleteAsync(ct);

            return Ok();
        }

        // Brise jednu stavku (X ikonica). Tudji ili nepostojeci Id se tiho ignorise (isto "Ok").
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> RemoveSearch(long id, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            await _context.UserSearches
                .Where(s => s.Id == id && s.UserId == userId.Value)
                .ExecuteDeleteAsync(ct);

            return Ok();
        }

        // "Clear all" - brise celu istoriju pretrage ovog korisnika.
        [HttpDelete]
        public async Task<IActionResult> ClearSearches(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            await _context.UserSearches
                .Where(s => s.UserId == userId.Value)
                .ExecuteDeleteAsync(ct);

            return Ok();
        }

        private long? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var id) ? id : null;
        }
    }
}
