using DataAccess;
using Domain.Entities;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;
using WebAPI.DTO.Input;
using WebAPI.DTO.Output;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    // Edit profile: promena korisnickog imena i e-mail adrese ulogovanog korisnika. Obe promene upisuju UpdatedAt.
    // E-mail se menja u dva koraka: (1) zahtev (uz trenutnu lozinku) -> sestocifreni kod stize na NOVU adresu; (2) potvrda koda
    // -> adresa se menja, a na STARU adresu stize obavestenje o promeni.
    [ApiController]
    [Route("api/users/me")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        // Isto pravilo kao pri registraciji (RegisterViewModel): najmanje 3 znaka, mala slova, cifre i @ _ .
        private static readonly Regex UsernamePattern = new(@"^[a-z0-9@_.]{3,}$", RegexOptions.Compiled);

        private readonly ScoreHubDbContext _context;
        private readonly IVerificationCodeService _codes;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly int _restoreGraceDays;

        public AccountController(ScoreHubDbContext context, IVerificationCodeService codes, IEmailSender emailSender,
            ILogger<AccountController> logger, IConfiguration configuration)
        {
            _context = context;
            _codes = codes;
            _emailSender = emailSender;
            _logger = logger;
            _restoreGraceDays = AccountRestore.GraceDays(configuration);
        }

        // Soft delete naloga: IsDeleted = true (red ostaje - prognoze i istorija), a ODMAH se prekida sve sto veze nalog za
        // svet: sesije (refresh tokeni), push uredjaji i prijateljstva/zahtevi. Posle toga se ne moze ulogovati, ne vidi se u
        // pretrazi ni na profilima. Pristupni token koji je vec izdat vazi jos najvise nekoliko minuta (TokenExpirationMinutes).
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value && !u.IsDeleted, ct);
            if (user is null)
                return Unauthorized();

            var now = DateTime.UtcNow;
            user.IsDeleted = true;
            user.DeletedAt = now;
            user.UpdatedAt = now;

            foreach (var jwt in await _context.Jwt.Where(j => j.UserId == user.Id && !j.IsRevoked).ToListAsync(ct))
                jwt.IsRevoked = true;

            _context.DeviceTokens.RemoveRange(await _context.DeviceTokens.Where(d => d.UserId == user.Id).ToListAsync(ct));

            _context.Friendships.RemoveRange(await _context.Friendships
                .Where(f => f.RequesterId == user.Id || f.AddresseeId == user.Id)
                .ToListAsync(ct));

            await _context.SaveChangesAsync(ct);

            // Potvrda na e-mail - ne sme da obori uspesno brisanje.
            try
            {
                var (subject, html, text) = EmailTemplates.AccountDeletedNotice(user.Username, now, now.AddDays(_restoreGraceDays));
                await _emailSender.SendAsync(user.Email, subject, html, text, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slanje potvrde o brisanju naloga nije uspelo (korisnik {UserId}).", user.Id);
            }

            return Ok();
        }

        [HttpPut("username")]
        public async Task<ActionResult<UsernameDTO>> UpdateUsername([FromBody] UpdateUsernameRequestDTO request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var username = request.Username.Trim();

            if (!UsernamePattern.IsMatch(username))
                return BadRequest(new ErrorResponseDTO { Errors = { "Username must be at least 3 characters and can only contain lowercase English letters, digits, and the characters @ _ ." } });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value && !u.IsDeleted, ct);
            if (user is null)
                return Unauthorized();

            if (user.Username == username)
                return Ok(new UsernameDTO { Username = user.Username });

            if (await _context.Users.AnyAsync(u => u.Username == username && u.Id != user.Id, ct))
                return Conflict(new ErrorResponseDTO { Errors = { "This username is already taken." } });

            user.Username = username;
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                // Dve istovremene promene na isto ime (unique indeks) - druga gubi.
                return Conflict(new ErrorResponseDTO { Errors = { "This username is already taken." } });
            }

            return Ok(new UsernameDTO { Username = user.Username });
        }

        [HttpPost("email/request")]
        public async Task<ActionResult<CodeSentDTO>> RequestEmailChange([FromBody] RequestEmailChangeDTO request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            if (PasswordAttemptLimiter.IsLockedOut(userId.Value))
                return StatusCode(StatusCodes.Status429TooManyRequests, new ErrorResponseDTO { Errors = { "Too many incorrect attempts. Please try again in a few minutes." } });

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId.Value && !u.IsDeleted, ct);
            if (user is null)
                return Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            {
                PasswordAttemptLimiter.RegisterFailure(userId.Value);
                return BadRequest(new ErrorResponseDTO { Errors = { "Current password is incorrect." } });
            }

            var newEmail = request.NewEmail.Trim();

            if (newEmail.Length > 256 || !new EmailAddressAttribute().IsValid(newEmail))
                return BadRequest(new ErrorResponseDTO { Errors = { "Please enter a valid email address." } });

            if (string.Equals(newEmail, user.Email, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new ErrorResponseDTO { Errors = { "This is already your email address." } });

            if (await _context.Users.AnyAsync(u => u.Email == newEmail && u.Id != user.Id, ct))
                return Conflict(new ErrorResponseDTO { Errors = { "This email address is already in use." } });

            PasswordAttemptLimiter.Reset(userId.Value);

            return ToResponse(await _codes.IssueAsync(userId.Value, VerificationPurpose.EmailChange, newEmail, ct), newEmail);
        }

        // "Resend code" - isti zahtev, NOV kod na istu (novu) adresu; pauza i dnevno ogranicenje vazi u servisu.
        [HttpPost("email/resend")]
        public async Task<ActionResult<CodeSentDTO>> ResendEmailCode(CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var target = await _codes.GetPendingTargetAsync(userId.Value, VerificationPurpose.EmailChange, ct);
            if (target is null)
                return BadRequest(new ErrorResponseDTO { Errors = { "There is no pending email change. Please start again." } });

            return ToResponse(await _codes.IssueAsync(userId.Value, VerificationPurpose.EmailChange, target, ct), target);
        }

        [HttpPost("email/confirm")]
        public async Task<ActionResult<EmailDTO>> ConfirmEmailChange([FromBody] ConfirmEmailCodeDTO request, CancellationToken ct)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _codes.VerifyAsync(userId.Value, VerificationPurpose.EmailChange, request.Code, ct);

            switch (result.Status)
            {
                case VerifyStatus.NotFound:
                    return BadRequest(new ErrorResponseDTO { Errors = { "There is no pending email change. Please start again." } });
                case VerifyStatus.Expired:
                    return BadRequest(new ErrorResponseDTO { Errors = { "This code has expired. Please request a new one." } });
                case VerifyStatus.TooManyAttempts:
                    return StatusCode(StatusCodes.Status429TooManyRequests, new ErrorResponseDTO { Errors = { "Too many incorrect codes. Please request a new one." } });
                case VerifyStatus.Invalid:
                    return BadRequest(new ErrorResponseDTO { Errors = { $"Incorrect code. {result.AttemptsLeft} attempt(s) left." } });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value && !u.IsDeleted, ct);
            if (user is null)
                return Unauthorized();

            var newEmail = result.TargetEmail!;

            // Adresu je u medjuvremenu mogao da zauzme neko drugi.
            if (await _context.Users.AnyAsync(u => u.Email == newEmail && u.Id != user.Id, ct))
                return Conflict(new ErrorResponseDTO { Errors = { "This email address is already in use." } });

            var oldEmail = user.Email;
            user.Email = newEmail;
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateException)
            {
                return Conflict(new ErrorResponseDTO { Errors = { "This email address is already in use." } });
            }

            // Obavestenje na STARU adresu - ako to nisi bio ti, ovako saznajes. Ne sme da obori uspesnu promenu.
            try
            {
                var (subject, html, text) = EmailTemplates.EmailChangedNotice(EmailTemplates.MaskEmail(newEmail), DateTime.UtcNow);
                await _emailSender.SendAsync(oldEmail, subject, html, text, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Slanje obavestenja o promeni e-maila na staru adresu nije uspelo (korisnik {UserId}).", user.Id);
            }

            return Ok(new EmailDTO { Email = newEmail });
        }

        private ActionResult<CodeSentDTO> ToResponse(IssueResult result, string targetEmail) => result.Status switch
        {
            IssueStatus.Sent => Ok(new CodeSentDTO
            {
                ExpiresInSeconds = result.ExpiresInSeconds,
                ResendAfterSeconds = result.ResendAfterSeconds,
                SentTo = EmailTemplates.MaskEmail(targetEmail)
            }),
            IssueStatus.TooSoon => StatusCode(StatusCodes.Status429TooManyRequests,
                new ErrorResponseDTO { Errors = { $"Please wait {result.RetryAfterSeconds} seconds before requesting a new code." } }),
            IssueStatus.LimitReached => StatusCode(StatusCodes.Status429TooManyRequests,
                new ErrorResponseDTO { Errors = { "Too many codes requested. Please try again in an hour." } }),
            _ => StatusCode(StatusCodes.Status502BadGateway,
                new ErrorResponseDTO { Errors = { "We could not send the email. Please try again later." } })
        };

        private long? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(claim, out var id) ? id : null;
        }
    }
}
