namespace MAUI.DTO.Input
{
    public class UsernameDTO
    {
        public string Username { get; set; } = string.Empty;
    }

    // Odgovor na "kod poslat": rok vazenja koda, pauza do sledeceg "Resend" i MASKIRANA adresa (j***@gmail.com).
    public class CodeSentDTO
    {
        public int ExpiresInSeconds { get; set; }
        public int ResendAfterSeconds { get; set; }
        public string SentTo { get; set; } = string.Empty;
    }

    public class ResetTokenDTO
    {
        public string ResetToken { get; set; } = string.Empty;
    }

    public class EmailDTO
    {
        public string Email { get; set; } = string.Empty;
    }
}
