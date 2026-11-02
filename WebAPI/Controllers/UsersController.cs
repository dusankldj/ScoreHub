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
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public UsersController(ScoreHubDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDTO>> GetMe(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !long.TryParse(userId, out var userID))
                return Unauthorized();

            var user = await _context.Users
                .Where(u => u.Id == userID && !u.IsDeleted)
                .Select(u => new UserProfileDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (user is null)
                return Unauthorized();

            return Ok(user);
        }

        [HttpGet("me/favorite-teams")]
        public async Task<ActionResult<List<FavoriteTeamDTO>>> GetMyFavoriteTeams(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !long.TryParse(userId, out var userID))
                return Unauthorized();

            var favorites = await _context.FavoriteTeams
                .Where(f => f.UserId == userID)
                .Select(f => new FavoriteTeamDTO
                {
                    TeamId = f.TeamId,
                    TeamName = f.Team.Name,
                    ShortName = f.Team.ShortName,
                    LogoUrl = f.Team.LogoUrl,
                    SportName = f.Team.League.Sport.Name
                })
                .ToListAsync(ct);

            return Ok(favorites);
        }


        // Pretraga korisnika po korisnickom imenu (za "Search user" na Profile stranici): bez sebe, najvise 20,
        // samo Id + Username. Minimalna duzina upita (2) se proverava i na klijentu - ovde je samo zastita.
        [HttpGet("search")]
        public async Task<ActionResult<List<UserSearchResultDTO>>> SearchUsers([FromQuery] string query, CancellationToken ct)
        {
            var me = GetUserId();
            if (me is null)
                return Unauthorized();

            var trimmed = query?.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.Length < 2)
                return Ok(new List<UserSearchResultDTO>());

            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id != me.Value && !u.IsDeleted && u.Username.Contains(trimmed))
                .OrderBy(u => u.Username)
                .Take(20)
                .Select(u => new UserSearchResultDTO { Id = u.Id, Username = u.Username })
                .ToListAsync(ct);

            return Ok(users);
        }

        // Tudji profil - javno samo username + member since + odnos prema ulogovanom korisniku.
        [HttpGet("{id:long}")]
        public async Task<ActionResult<UserPageDTO>> GetUser(long id, CancellationToken ct)
        {
            var me = GetUserId();
            if (me is null)
                return Unauthorized();

            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id && !u.IsDeleted)
                .Select(u => new { u.Id, u.Username, u.FirstName, u.LastName, MemberSince = u.CreatedAt })
                .FirstOrDefaultAsync(ct);

            if (user is null)
                return NotFound();

            var relationship = await _context.GetRelationshipAsync(me.Value, id, ct);

            return Ok(new UserPageDTO
            {
                Id = user.Id,
                Username = user.Username,
                MemberSince = user.MemberSince,
                Relationship = relationship,
                // Ime i prezime vide samo prijatelji (ostali samo korisnicko ime i datum registracije).
                FullName = relationship == FriendshipHelper.Friends ? $"{user.FirstName} {user.LastName}".Trim() : null
            });
        }

        // Omiljeni timovi drugog korisnika - SAMO za prijatelje (inace 403).
        [HttpGet("{id:long}/favorite-teams")]
        public async Task<ActionResult<List<FavoriteTeamDTO>>> GetUserFavoriteTeams(long id, CancellationToken ct)
        {
            var me = GetUserId();
            if (me is null)
                return Unauthorized();

            if (me.Value != id && !await _context.AreFriendsAsync(me.Value, id, ct))
                return Forbid();

            var favorites = await _context.FavoriteTeams
                .AsNoTracking()
                .Where(f => f.UserId == id)
                .Select(f => new FavoriteTeamDTO
                {
                    TeamId = f.TeamId,
                    TeamName = f.Team.Name,
                    ShortName = f.Team.ShortName,
                    LogoUrl = f.Team.LogoUrl,
                    SportName = f.Team.League.Sport.Name
                })
                .ToListAsync(ct);

            return Ok(favorites);
        }

        private long? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var id) ? id : null;
        }

        [HttpPost("me/device-token")]
        public async Task<IActionResult> RegisterDeviceToken([FromBody] DeviceTokenDTO request, CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !long.TryParse(userId, out var userID))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request?.Token))
                return BadRequest();

            var existing = await _context.DeviceTokens.FirstOrDefaultAsync(d => d.Token == request.Token, ct);

            if (existing is not null)
                existing.UserId = userID;
            else
                _context.DeviceTokens.Add(new DeviceToken { UserId = userID, Token = request.Token });

            await _context.SaveChangesAsync(ct);
            return Ok();
        }

        [HttpDelete("me/device-token")]
        public async Task<IActionResult> UnregisterDeviceToken([FromBody] DeviceTokenDTO request, CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !long.TryParse(userId, out var userID))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request?.Token))
                return BadRequest();

            var existing = await _context.DeviceTokens.FirstOrDefaultAsync(d => d.Token == request.Token && d.UserId == userID, ct);
            if (existing is not null)
            {
                _context.DeviceTokens.Remove(existing);
                await _context.SaveChangesAsync(ct);
            }

            return Ok();
        }
    }
}
