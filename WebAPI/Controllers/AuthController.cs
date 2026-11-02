using DataAccess;
using Domain.Entities;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebAPI.DTO.Input;
using WebAPI.DTO.Output;
using WebAPI.Helpers;
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

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuthController> _logger;
        private readonly int _restoreGraceDays;

        public AuthController(ScoreHubDbContext context, IJwtService jwtService, IOptions<JwtOptions> jwtOptions,
            IServiceScopeFactory scopeFactory, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _context = context;
            _jwtService = jwtService;
            _jwtOptions = jwtOptions.Value;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _restoreGraceDays = AccountRestore.GraceDays(configuration);
        }

        // Prijava. Obrisan (soft delete) nalog se ovde VRACA: ispravno korisnicko ime + lozinka u roku od
        // Account:RestoreGraceDays dana od brisanja -> IsDeleted = false i nalog radi dalje (prognoze i omiljeni timovi su
        // ostali; prijateljstva su prekinuta pri brisanju). Posle roka, ili uz pogresnu lozinku, odgovor je isti kao za
        // nepostojeci nalog - ne otkriva se da je nalog obrisan.
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

            if (!validPass || (user.IsDeleted && !AccountRestore.IsWithinWindow(user, _restoreGraceDays)))
                return Unauthorized(new ErrorResponseDTO
                {
                    Errors = new List<string> { "Incorrect username or password." }
                });

            var restored = user.IsDeleted;
            if (restored)
            {
                user.IsDeleted = false;
                user.DeletedAt = null;
                user.UpdatedAt = DateTime.UtcNow;
            }


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

            if (restored)
                SendEmailInBackground(user.Email, EmailTemplates.AccountRestoredNotice(user.Username, DateTime.UtcNow), $"account restored ({user.Username})");

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

            if(user is null || user.IsDeleted)
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

            SendEmailInBackground(newUser.Email, EmailTemplates.Welcome(newUser.FirstName, newUser.Username), $"welcome ({newUser.Username})");

            return Ok();
        }

        // Mejl se salje u pozadini (sopstveni DI scope, jer se zahtev zavrsava pre slanja): zahtev ne ceka SMTP i NE pada ako
        // slanje ne uspe (greska se samo loguje). Prima gotove vrednosti, ne entitet - nema pristupa DbContext-u.
        private void SendEmailInBackground(string toAddress, (string Subject, string Html, string Text) mail, string what)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    await sender.SendAsync(toAddress, mail.Subject, mail.Html, mail.Text);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Slanje mejla ({What}) nije uspelo.", what);
                }
            });
        }


        // ===================== Forgot password (3 koraka, bez prijave) =====================
        // 1) forgot-password: e-mail -> sestocifreni kod stize na adresu NALOGA (ako nalog postoji).
        // 2) forgot-password/verify: e-mail + kod -> jednokratni reset token.
        // 3) reset-password: e-mail + reset token + nova lozinka -> lozinka se menja, SVE sesije se opozivaju.
        // Odgovori ne odaju da li nalog postoji: korak 1 UVEK vraca isti uspeh (sav rad ide u pozadini, pa je i vreme odgovora
        // isto), a koraci 2 i 3 za sve neuspehe vracaju istu opstu poruku.
        private const string GenericCodeError = "Incorrect or expired code. Please try again or request a new one.";
        private static readonly TimeSpan ResetTokenLifetime = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ThrottleWindow = TimeSpan.FromMinutes(15);

        private bool ThrottleExceeded(string email, int maxPerIp, int maxPerEmail)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var ipOk = RequestThrottle.Allow($"reset:ip:{ip}", maxPerIp, ThrottleWindow);
            var emailOk = RequestThrottle.Allow($"reset:email:{email.ToLowerInvariant()}", maxPerEmail, ThrottleWindow);
            return !(ipOk && emailOk);
        }

        private static bool IsValidEmail(string email)
            => email.Length <= 256 && new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email);

        [HttpPost("forgot-password")]
        public ActionResult ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            var email = request.Email.Trim();

            if (!IsValidEmail(email))
                return BadRequest(new ErrorResponseDTO { Errors = new List<string> { "Please enter a valid email address." } });

            if (ThrottleExceeded(email, maxPerIp: 10, maxPerEmail: 5))
                return StatusCode(StatusCodes.Status429TooManyRequests, new ErrorResponseDTO
                {
                    Errors = new List<string> { "Too many requests. Please try again in a few minutes." }
                });

            // Sve ostalo (trazenje naloga, pravljenje koda, slanje mejla) ide u pozadini - odgovor je isti i trenutan.
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ScoreHubDbContext>();
                    var codes = scope.ServiceProvider.GetRequiredService<IVerificationCodeService>();

                    var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
                    if (user is not null)
                        await codes.IssueAsync(user.Id, VerificationPurpose.PasswordReset, user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Forgot password: slanje koda nije uspelo.");
                }
            });

            return Ok(new CodeSentDTO
            {
                ExpiresInSeconds = (int)VerificationCodeService.CodeLifetime.TotalSeconds,
                ResendAfterSeconds = (int)VerificationCodeService.ResendCooldown.TotalSeconds,
                SentTo = EmailTemplates.MaskEmail(email)
            });
        }

        [HttpPost("forgot-password/verify")]
        public async Task<ActionResult> VerifyResetCode([FromBody] VerifyResetCodeDTO request, CancellationToken ct)
        {
            var email = request.Email.Trim();

            if (ThrottleExceeded(email, maxPerIp: 30, maxPerEmail: 15))
                return StatusCode(StatusCodes.Status429TooManyRequests, new ErrorResponseDTO
                {
                    Errors = new List<string> { "Too many attempts. Please try again in a few minutes." }
                });

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, ct);

            if (user is null)
                return BadRequest(new ErrorResponseDTO { Errors = new List<string> { GenericCodeError } });

            var codes = HttpContext.RequestServices.GetRequiredService<IVerificationCodeService>();
            var result = await codes.VerifyAsync(user.Id, VerificationPurpose.PasswordReset, request.Code, ct);

            if (result.Status != VerifyStatus.Success)
                return BadRequest(new ErrorResponseDTO { Errors = new List<string> { GenericCodeError } });

            var token = await codes.CreateTokenAsync(user.Id, VerificationPurpose.PasswordResetToken, user.Email, ResetTokenLifetime, ct);
            return Ok(new ResetTokenDTO { ResetToken = token });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request, CancellationToken ct)
        {
            var email = request.Email.Trim();
            const string expired = "This reset session has expired. Please start again.";

            if (ThrottleExceeded(email, maxPerIp: 30, maxPerEmail: 15))
                return StatusCode(StatusCodes.Status429TooManyRequests, new ErrorResponseDTO
                {
                    Errors = new List<string> { "Too many attempts. Please try again in a few minutes." }
                });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, ct);

            if (user is null)
                return BadRequest(new ErrorResponseDTO { Errors = new List<string> { expired } });

            var codes = HttpContext.RequestServices.GetRequiredService<IVerificationCodeService>();
            var result = await codes.VerifyAsync(user.Id, VerificationPurpose.PasswordResetToken, request.ResetToken, ct);

            if (result.Status != VerifyStatus.Success)
                return BadRequest(new ErrorResponseDTO { Errors = new List<string> { expired } });

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Opoziv SVIH sesija - ko je imao pristup nalogu (ili ukraden token) mora ponovo da se prijavi novom lozinkom.
            foreach (var jwt in await _context.Jwt.Where(j => j.UserId == user.Id && !j.IsRevoked).ToListAsync(ct))
                jwt.IsRevoked = true;

            await _context.SaveChangesAsync(ct);
            PasswordAttemptLimiter.Reset(user.Id);

            SendEmailInBackground(user.Email, EmailTemplates.PasswordChangedNotice(DateTime.UtcNow), $"password reset ({user.Username})");

            return Ok();
        }

        // Promena lozinke ulogovanog korisnika. Proverava trenutnu lozinku (BCrypt, kao login), upisuje BCrypt hash nove i
        // rotira tokene: ovaj uredjaj dobija NOVE tokene, a stari refresh token (npr. sa drugog uredjaja ili ukraden) vise ne
        // vazi - sistem cuva jedan aktivan refresh token po korisniku. Pogresna trenutna lozinka vraca 400, ne 401, jer
        // klijent na 401 pokusava obnovu tokena.
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request, CancellationToken ct)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claim is null || !long.TryParse(claim, out var userId))
                return Unauthorized();

            if (PasswordAttemptLimiter.IsLockedOut(userId))
                return StatusCode(StatusCodes.Status429TooManyRequests, new ErrorResponseDTO
                {
                    Errors = new List<string> { "Too many incorrect attempts. Please try again in a few minutes." }
                });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct);

            if (user is null)
                return Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            {
                PasswordAttemptLimiter.RegisterFailure(userId);
                return BadRequest(new ErrorResponseDTO { Errors = new List<string> { "Current password is incorrect." } });
            }

            if (request.CurrentPassword == request.NewPassword)
                return BadRequest(new ErrorResponseDTO { Errors = new List<string> { "New password must be different from the current one." } });

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var existingJwt = await _context.Jwt.FirstOrDefaultAsync(j => j.UserId == userId && !j.IsRevoked, ct);

            if (existingJwt is not null)
            {
                existingJwt.Token = token;
                existingJwt.RefreshJwtToken = refreshToken;
                existingJwt.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.Jwt.Add(new Jwt { UserId = userId, Token = token, RefreshJwtToken = refreshToken, IsRevoked = false });
            }

            await _context.SaveChangesAsync(ct);
            PasswordAttemptLimiter.Reset(userId);

            // Obavestenje o promeni lozinke na e-mail naloga - ako to nisi bio ti, tako saznajes.
            SendEmailInBackground(user.Email, EmailTemplates.PasswordChangedNotice(DateTime.UtcNow), $"password changed ({user.Username})");

            return Ok(new AuthResponseDTO { Token = token, RefreshToken = refreshToken });
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
