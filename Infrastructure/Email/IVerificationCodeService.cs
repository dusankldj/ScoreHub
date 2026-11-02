using Domain.Entities;

namespace Infrastructure.Email
{
    public enum IssueStatus
    {
        Sent,
        // Resend prerano - mora da prodje pauza (ResendCooldown).
        TooSoon,
        // Previse kodova za sat vremena.
        LimitReached,
        // SMTP greska - kod nije poslat (i ne racuna se u ogranicenja).
        SendFailed
    }

    public enum VerifyStatus
    {
        Success,
        Invalid,
        Expired,
        TooManyAttempts,
        NotFound
    }

    public sealed record IssueResult(IssueStatus Status, int RetryAfterSeconds = 0, int ExpiresInSeconds = 0, int ResendAfterSeconds = 0);

    public sealed record VerifyResult(VerifyStatus Status, string? TargetEmail = null, int AttemptsLeft = 0);

    public interface IVerificationCodeService
    {
        // Pravi NOV sestocifreni kod (poništava prethodni), cuva njegov heš i salje ga na targetEmail.
        Task<IssueResult> IssueAsync(long userId, VerificationPurpose purpose, string targetEmail, CancellationToken ct = default);

        // Jednokratni token (npr. reset token). Proverava se sa VerifyAsync (token umesto koda).
        Task<string> CreateTokenAsync(long userId, VerificationPurpose purpose, string targetEmail, TimeSpan lifetime, CancellationToken ct = default);

        // Adresa poslednjeg (neiskoriscenog) koda za "Resend" - null ako korisnik nema nezavrsen zahtev.
        Task<string?> GetPendingTargetAsync(long userId, VerificationPurpose purpose, CancellationToken ct = default);

        Task<VerifyResult> VerifyAsync(long userId, VerificationPurpose purpose, string code, CancellationToken ct = default);
    }
}
