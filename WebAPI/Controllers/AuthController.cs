using DataAccess;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebAPI.DTO.Input;
using WebAPI.DTO.Output;
using WebAPI.JWT;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly JwtOptions _jwtOptions;

        public AuthController(ScoreHubDbContext context, IJwtService jwtService, IOptions<JwtOptions> jwtOptions)
        {
            _context = context;
            _jwtService = jwtService;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken ct)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username, ct);

            if (user == null)
                return Unauthorized(new ErrorResponseDTO
                {
                    Errors = new List<string> { "Incorrect username or password." }
                });

            bool validPass = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!validPass)
                return Unauthorized(new ErrorResponseDTO
                {
                    Errors = new List<string> { "Incorrect username or password." }
                });


            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();


            var existingJwt = await _context.Jwt.FirstOrDefaultAsync(j => j.UserId == user.Id && !j.IsRevoked, ct);

            if (existingJwt is not null)
            {
                existingJwt.Token = token;
                existingJwt.RefreshJwtToken = refreshToken;
            }else{
                _context.Jwt.Add(new Jwt
                {
                    UserId = user.Id,
                    Token = token,
                    RefreshJwtToken = refreshToken,
                    IsRevoked = false
                });
            }

            await _context.SaveChangesAsync(ct);

            return Ok(new AuthResponseDTO
            {
                Token = token, 
                RefreshToken = refreshToken
            });
        }


        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshRequestDTO request, CancellationToken ct)
        {
            var existingJwt = await _context.Jwt.FirstOrDefaultAsync(j => j.RefreshJwtToken == request.RefreshToken, ct);

            if(existingJwt is null || existingJwt.IsRevoked)
                return Unauthorized(new ErrorResponseDTO
                {
                    Errors = new List<string> { "Invalid or expired refresh token." }
                });

            var lastIssuedAt = existingJwt.UpdatedAt ?? existingJwt.CreatedAt;

            var expired = lastIssuedAt.AddHours(_jwtOptions.RefreshTokenExpirationHours) < DateTime.UtcNow;

            if (expired)
            {
                existingJwt.IsRevoked = true;
                await _context.SaveChangesAsync(ct);

                return Unauthorized(new ErrorResponseDTO
                {
                    Errors = new List<string> { "Invalid or expired refresh token." }
                });
            }

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == existingJwt.UserId, ct);

            if(user is null)
                return Unauthorized(new ErrorResponseDTO
                {
                    Errors = new List<string> { "Invalid or expired refresh token." }
                });


            var newToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            existingJwt.Token = newToken;
            existingJwt.RefreshJwtToken = newRefreshToken;
            existingJwt.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);

            return Ok(new AuthResponseDTO
            {
                Token = newToken,
                RefreshToken = newRefreshToken
            });
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequestDTO request, CancellationToken ct)
        {
            User user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username ||u.Email == request.Email, ct);


            var errors = new List<string>();

            if (user is not null)
            {
                if (user.Username == request.Username)
                    errors.Add("Username already exists. Try another one.");

                if (user.Email == request.Email)
                    errors.Add("Email already in use.");
            }

            if (errors.Count > 0)
                return BadRequest(new ErrorResponseDTO { Errors = errors });


            string hashedPass = BCrypt.Net.BCrypt.HashPassword(request.Password);

            User newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName, 
                Username = request.Username,
                Email = request.Email,
                Password = hashedPass,
                RoleId = 1
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout(CancellationToken ct)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null || !long.TryParse(userId, out var userID))
                return Unauthorized();


            var existingJwt = await _context.Jwt.FirstOrDefaultAsync(j => j.UserId == userID && !j.IsRevoked, ct);

            if (existingJwt is not null)    
            {
                existingJwt.IsRevoked = true;
                await _context.SaveChangesAsync(ct);
            }

            return Ok();
        }
    }
}
