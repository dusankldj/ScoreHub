using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Email
{
    // Sestocifreni kod: generise se kriptografski slucajno (RandomNumberGenerator), vazi CodeLifetime, "Resend" je moguc tek
    // posle ResendCooldown, najvise MaxSendsPerHour kodova na sat, najvise MaxFailedAttempts pogresnih unosa po kodu.
    // U bazi je samo salted SHA-256 heš (nikad sam kod).
    public class VerificationCodeService : IVerificationCodeService
    {
        public static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);
        public const int MaxSendsPerHour = 5;
        public const int MaxFailedAttempts = 5;

        private readonly ScoreHubDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<VerificationCodeService> _logger;

        public VerificationCodeService(ScoreHubDbContext context, IEmailSender emailSender, ILogger<VerificationCodeService> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<IssueResult> IssueAsync(long userId, VerificationPurpose purpose, string targetEmail, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var recent = await _context.EmailVerificationCodes
                .AsNoTracking()
                .Where(c => c.UserId == userId && c.Purpose == purpose && c.CreatedAt > now.AddHours(-1))
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => c.CreatedAt)
                .ToListAsync(ct);

            if (recent.Count > 0)
            {
                var elapsed = now - recent[0];
                if (elapsed < ResendCooldown)
                    return new IssueResult(IssueStatus.TooSoon, RetryAfterSeconds: (int)Math.Ceiling((ResendCooldown - elapsed).TotalSeconds));
            }

            if (recent.Count >= MaxSendsPerHour)
                return new IssueResult(IssueStatus.LimitReached);

            // Prethodni nezavrseni kodovi vise ne vaze - vazi samo poslednji poslat.
            var active = await _context.EmailVerificationCodes
                .Where(c => c.UserId == userId && c.Purpose == purpose && c.ConsumedAt == null && c.ExpiresAt > now)
                .ToListAsync(ct);
            foreach (var old in active)
                old.ConsumedAt = now;

            var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

            var entity = new EmailVerificationCode
            {
                UserId = userId,
                Purpose = purpose,
                TargetEmail = targetEmail,
                CodeHash = Hash(code),
                ExpiresAt = now.Add(CodeLifetime)
            };
            _context.EmailVerificationCodes.Add(entity);
            await _context.SaveChangesAsync(ct);

            try
            {
                var (subject, html, text) = EmailTemplates.VerificationCode(code, (int)CodeLifetime.TotalMinutes, purpose);
                await _emailSender.SendAsync(targetEmail, subject, html, text, ct);
            }
            catch (Exception ex)
            {
                // Poruka nije stigla - kod se brise i NE racuna u pauzu/ogranicenje, da korisnik odmah moze ponovo.
                _logger.LogError(ex, "Slanje verifikacionog koda nije uspelo (korisnik {UserId}).", userId);
                _context.EmailVerificationCodes.Remove(entity);
                foreach (var old in active)
                    old.ConsumedAt = null;
                await _context.SaveChangesAsync(CancellationToken.None);
                return new IssueResult(IssueStatus.SendFailed);
            }

            return new IssueResult(IssueStatus.Sent,
                ExpiresInSeconds: (int)CodeLifetime.TotalSeconds,
                ResendAfterSeconds: (int)ResendCooldown.TotalSeconds);
        }

        // Jednokratni token (npr. reset token posle ispravnog koda): 32 slucajna bajta, u bazi samo salted heš, vazi `lifetime`.
        // Novi token poništava prethodni iste svrhe. Proverava se istim VerifyAsync-om (token umesto koda).
        public async Task<string> CreateTokenAsync(long userId, VerificationPurpose purpose, string targetEmail, TimeSpan lifetime, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var active = await _context.EmailVerificationCodes
                .Where(c => c.UserId == userId && c.Purpose == purpose && c.ConsumedAt == null && c.ExpiresAt > now)
                .ToListAsync(ct);
            foreach (var old in active)
                old.ConsumedAt = now;

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace('+', '-').Replace('/', '_').TrimEnd('=');

            _context.EmailVerificationCodes.Add(new EmailVerificationCode
            {
                UserId = userId,
                Purpose = purpose,
                TargetEmail = targetEmail,
                CodeHash = Hash(token),
                ExpiresAt = now.Add(lifetime)
            });
            await _context.SaveChangesAsync(ct);

            return token;
        }

        public async Task<string?> GetPendingTargetAsync(long userId, VerificationPurpose purpose, CancellationToken ct = default)
        {
            var since = DateTime.UtcNow.AddHours(-1);

            return await _context.EmailVerificationCodes
                .AsNoTracking()
                .Where(c => c.UserId == userId && c.Purpose == purpose && c.ConsumedAt == null && c.CreatedAt > since)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => c.TargetEmail)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<VerifyResult> VerifyAsync(long userId, VerificationPurpose purpose, string code, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var entity = await _context.EmailVerificationCodes
                .Where(c => c.UserId == userId && c.Purpose == purpose && c.ConsumedAt == null)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if (entity is null)
                return new VerifyResult(VerifyStatus.NotFound);

            if (entity.ExpiresAt <= now)
                return new VerifyResult(VerifyStatus.Expired);

            if (entity.FailedAttempts >= MaxFailedAttempts)
                return new VerifyResult(VerifyStatus.TooManyAttempts);

            if (!Matches(code, entity.CodeHash))
            {
                entity.FailedAttempts++;
                await _context.SaveChangesAsync(ct);

                return entity.FailedAttempts >= MaxFailedAttempts
                    ? new VerifyResult(VerifyStatus.TooManyAttempts)
                    : new VerifyResult(VerifyStatus.Invalid, AttemptsLeft: MaxFailedAttempts - entity.FailedAttempts);
            }

            entity.ConsumedAt = now;
            await _context.SaveChangesAsync(ct);
            return new VerifyResult(VerifyStatus.Success, entity.TargetEmail);
        }

        // "salt:hash" - salt je slucajan po kodu, heš = SHA-256(salt + kod). Poredjenje je u konstantnom vremenu.
        private static string Hash(string code)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            return Convert.ToHexString(salt) + ":" + Convert.ToHexString(Compute(salt, code));
        }

        private static bool Matches(string code, string stored)
        {
            var parts = stored.Split(':');
            if (parts.Length != 2)
                return false;

            var expected = Convert.FromHexString(parts[1]);
            var actual = Compute(Convert.FromHexString(parts[0]), code.Trim());
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }

        private static byte[] Compute(byte[] salt, string code)
            => SHA256.HashData(salt.Concat(Encoding.UTF8.GetBytes(code)).ToArray());
    }
}
