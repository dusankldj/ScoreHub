using System.Globalization;

namespace MAUI.DTO.Input
{
    public class PlayerDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public string? BirthLocation { get; set; }
        public string? Team { get; set; }
        public string? SecondaryTeam { get; set; }
        public string? Position { get; set; }

        // "Denver Nuggets & Serbia Basketball" - igrac moze imati klub I reprezentaciju
        // istovremeno (SecondaryTeam je opcion, izostavlja se ako ne postoji).
        public string TeamDisplay => string.IsNullOrWhiteSpace(SecondaryTeam)
            ? Team ?? string.Empty
            : $"{Team} & {SecondaryTeam}";
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Weight { get; set; }
        public string? Height { get; set; }
        public string? Worth { get; set; }
        public string? FacebookLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? YoutubeLink { get; set; }
        public List<PlayerStatistics> Statistics { get; set; } = new();
        public List<PlayerHonours> Honours { get; set; } = new();
        public List<PlayerMilestones> Milestones { get; set; } = new();
        public List<PlayerFormerTeams> FormerTeams { get; set; } = new();

        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);

        public bool NoImage => !HasImage;

        public bool HasFacebook => !string.IsNullOrWhiteSpace(FacebookLink);

        public bool HasInstagram => !string.IsNullOrWhiteSpace(InstagramLink);

        public bool HasTwitter => !string.IsNullOrWhiteSpace(TwitterLink);

        public bool HasYoutube => !string.IsNullOrWhiteSpace(YoutubeLink);

        // YouTube namerno izostavljen - Socials tab vise ne prikazuje YouTube ikonicu.
        public bool HasAnySocial => HasFacebook || HasInstagram || HasTwitter;

        // Standardna formula - oduzima godinu pa vraca 1 nazad ako rodjendan ove godine jos
        // nije prosao.
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age))
                    age--;
                return age;
            }
        }

        // "FirstName LastName" - ako fali jedno od dva, ispisuje samo ono sto postoji.
        public string PlayerDisplay => (FirstName, LastName) switch
        {
            (var f, var l) when !string.IsNullOrWhiteSpace(f) && !string.IsNullOrWhiteSpace(l) => $"{f} {l}",
            (var f, _) when !string.IsNullOrWhiteSpace(f) => f!,
            (_, var l) when !string.IsNullOrWhiteSpace(l) => l!,
            _ => string.Empty
        };
    }

    public class PlayerStatistics
    {
        public string? Team { get; set; }
        public string? League { get; set; }
        public string Statistic { get; set; }
        public string Value { get; set; }
        public string Season { get; set; }

        public bool HasTeam => !string.IsNullOrWhiteSpace(Team);

        public bool HasLeague => !string.IsNullOrWhiteSpace(League);

        // Stat-ovi ciji naziv sadrzi "Percentage" (npr. "Field Goal Percentage") su u bazi
        // upisani kao decimalni razlomak (npr. ".378") - potvrdjeno u praksi (kosarkaski
        // procenti sutiranja). Ovde ih prikazujemo kao procenat (37.8%) umesto sirovog broja.
        public string ValueDisplay => Statistic.Contains("Percentage") && float.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var fraction)
            ? $"{(fraction * 100).ToString("0.#", CultureInfo.InvariantCulture)}%"
            : Value;

        public string StatDisplay => $"{Statistic}: {ValueDisplay}";

        // "Team (League)" - preskace deo koji nedostaje.
        public string SubText => (HasTeam, HasLeague) switch
        {
            (true, true) => $"{Team} ({League})",
            (true, false) => Team!,
            (false, true) => League!,
            _ => string.Empty
        };
    }

    public class PlayerHonours
    {
        public string Team { get; set; }
        public string League { get; set; }
        public string HonourName { get; set; }
        public string HonourTrophyUrl { get; set; }
        public string Season { get; set; }

        public bool HasTrophyImage => !string.IsNullOrWhiteSpace(HonourTrophyUrl);

        public bool HasTeam => !string.IsNullOrWhiteSpace(Team);

        public bool HasLeague => !string.IsNullOrWhiteSpace(League);

        // "Team • League • Season" - preskace deo koji nedostaje.
        public string SubText => string.Join(" • ", new[] { Team, HasLeague ? League : null, Season }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    public class PlayerMilestones
    {
        public string Team { get; set; }
        public string MilestoneName { get; set; }
        public string? MilestoneUrl { get; set; }
        public DateOnly DateAchieved { get; set; }

        public bool HasImage => !string.IsNullOrWhiteSpace(MilestoneUrl);

        public bool HasTeam => !string.IsNullOrWhiteSpace(Team);
    }

    public class PlayerFormerTeams
    {
        public string Team { get; set; }
        public string TeamLogoUrl { get; set; }
        public string MoveType { get; set; }
        public string Joined { get; set; }
        public string? Departed { get; set; }
        public string? Appearences { get; set; }
        public string? Goals { get; set; }

        public bool HasDeparted => !string.IsNullOrWhiteSpace(Departed);

        // "Joined - Departed" ili "Joined -" ako jos traje (Departed nepoznat/prazan).
        public string PeriodDisplay => $"{Joined} - {(HasDeparted ? Departed : string.Empty)}";

        public bool HasAppearances => !string.IsNullOrWhiteSpace(Appearences);

        public bool HasGoals => !string.IsNullOrWhiteSpace(Goals);
    }
}
