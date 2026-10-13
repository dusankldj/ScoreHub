namespace MAUI.DTO.Input
{
    public class LeagueDetailsDTO
    {
        public string Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Country { get; set; }
        public List<LeagueTeamDTO> Teams { get; set; } = new();
        public List<LeagueStandingDTO> Standings { get; set; } = new();

        public bool HasLogo => !string.IsNullOrWhiteSpace(LogoUrl);

        public bool NoLogo => !HasLogo;

        // Ime slike zastavice ("flag_rs") ili null - za nepoznatu/nedostajucu zemlju ILI generican
        // "World"/"Worldwide"/"International" (ne postoje u CountryFlagHelper mapi, pa vec vracaju
        // null bez ikakve posebne provere) - u tom slucaju se prikazuje ikonica planete.
        public string? FlagImage => Helpers.CountryFlagHelper.GetFlagImage(Country);

        public bool HasFlag => FlagImage is not null;

        public bool NoFlag => !HasFlag;
    }

    public class LeagueTeamDTO
    {
        public long TeamId { get; set; }
        public string LogoUrl { get; set; }
        public string TeamName { get; set; }
        public string? ShortName { get; set; }

        public bool HasShortName => !string.IsNullOrWhiteSpace(ShortName);
    }

    public class LeagueStandingDTO
    {
        public int Rank { get; set; }
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public string? TeamShortName { get; set; }
        public string TeamLogoUrl { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesDraw { get; set; }
        public int MatchesLost { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }
        public string? Description { get; set; }
        public string? Form { get; set; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

        public string TeamDisplayName => string.IsNullOrWhiteSpace(TeamShortName) ? TeamName : TeamShortName;

        // Popunjava LeagueViewModel posle ucitavanja (isti Description = ista boja kroz celu
        // tabelu) - ne dolazi sa API-ja, zato obican mutable setter umesto computed.
        public string? DescriptionColorHex { get; set; }

        private static readonly Color WinColor = Color.FromArgb("#4CAF50");
        private static readonly Color LossColor = Color.FromArgb("#D32F2F");
        private static readonly Color DrawColor = Colors.White;

        // "WWLDW" -> obojeni Span-ovi (W=zeleno, L=crveno, D=belo), bez razmaka izmedju slova.
        // Nepoznato slovo (retko, provajder ume da posalje i "-" za mec koji jos nije odigran u
        // formi) se ispisuje sivo, ne izostavlja se - da broj slova ostane tacan.
        public FormattedString FormDisplay
        {
            get
            {
                var formatted = new FormattedString();
                if (string.IsNullOrWhiteSpace(Form))
                    return formatted;

                foreach (var ch in Form.Trim())
                {
                    var color = char.ToUpperInvariant(ch) switch
                    {
                        'W' => WinColor,
                        'L' => LossColor,
                        'D' => DrawColor,
                        _ => Color.FromArgb("#8A8A8A")
                    };

                    formatted.Spans.Add(new Span { Text = ch.ToString(), TextColor = color });
                }

                return formatted;
            }
        }
    }
}
