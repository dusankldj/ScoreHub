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
                .Where(u => u.Id == userID)
                .Select(u => new UserProfileDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
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
