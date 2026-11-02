using System;

namespace Domain.Entities
{
    public enum VerificationPurpose
    {
        // Potvrda NOVE e-mail adrese pri promeni e-maila (Edit profile).
        EmailChange,

        // "Forgot password": sestocifreni kod poslat na e-mail naloga ...
        PasswordReset,

        // ... i JEDNOKRATNI reset token koji server izda kad je kod ispravan (korak 3 - nova lozinka, bez prijave).
        PasswordResetToken
    }

    /// <summary>
    /// Jednokratni sestocifreni kod poslat korisniku na e-mail. Cuva se SAMO heš koda (nikad sam kod). Kod vazi ograniceno
    /// vreme (ExpiresAt), ima ogranicen broj pogresnih unosa (FailedAttempts) i trosi se jednom (ConsumedAt). "Resend" pravi NOV
    /// kod i poništava prethodni. TargetEmail je adresa na koju je kod poslat (za promenu e-maila - NOVA adresa).
    /// </summary>
    public class EmailVerificationCode : BaseEntity.BaseEntity
    {
        public long UserId { get; set; }
        public VerificationPurpose Purpose { get; set; }
        public string TargetEmail { get; set; } = string.Empty;
        public string CodeHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? ConsumedAt { get; set; }

        public User User { get; set; }
    }
}
