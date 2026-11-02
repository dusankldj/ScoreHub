namespace WebAPI.DTO.Output
{
    public class UsernameDTO
    {
        public string Username { get; set; } = string.Empty;
    }

    // Odgovor na "kod poslat": koliko kod vazi i za koliko moze Resend; SentTo je MASKIRANA adresa (j***@gmail.com).
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
