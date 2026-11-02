namespace Infrastructure.Email
{
    // Podesavanja slanja e-maila (sekcija "Email"). Host/Port idu u appsettings.json, a ostalo u User Secrets:
    //   Email:Address      - Gmail adresa koja salje (SMTP login i "From" adresa)
    //   Email:AppPassword  - Gmail APP PASSWORD (ne obicna lozinka), 16 slova bez razmaka
    //   Email:Username     - ime posiljaoca koje primalac vidi (npr. "ScoreHub")
    public class EmailOptions
    {
        public const string SectionName = "Email";

        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;

        public string Address { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;

        // Ime posiljaoca ("ScoreHub <adresa@gmail.com>"); prazno = "ScoreHub".
        public string Username { get; set; } = string.Empty;

        public string DisplayName => string.IsNullOrWhiteSpace(Username) ? "ScoreHub" : Username;

        public bool IsConfigured => !string.IsNullOrWhiteSpace(Address) && !string.IsNullOrWhiteSpace(AppPassword);
    }
}
