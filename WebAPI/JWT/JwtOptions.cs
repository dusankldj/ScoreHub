namespace WebAPI.JWT
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } 
        public string Audience { get; set; }

        public int TokenExpirationMinutes { get; set; } = 15;
        public int RefreshTokenExpirationHours { get; set; } = 360;
    }
}
