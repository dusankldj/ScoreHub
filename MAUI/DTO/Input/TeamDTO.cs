namespace MAUI.DTO.Input
{
    public class TeamDTO
    {
        public string Name { get; set; }
        public string? ShortName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Stadium { get; set; }
        public string? Country { get; set; }
        public string? SportName { get; set; }
        public List<Squad> Squad { get; set; } = new();
        public List<TeamEquipment> Equipment { get; set; } = new();
        public List<TeamMatches> Matches { get; set; } = new();

        // "Ime (Skracenica)" - izostavlja zagradu ako ShortName ne postoji.
        public string NameDisplay => string.IsNullOrWhiteSpace(ShortName) ? Name : $"{Name} ({ShortName})";

        // Ime slike zastavice ("flag_rs") ili null - za nepoznatu/nedostajucu zemlju ILI generican
        // "World"/"Worldwide"/"International" (ne postoje u CountryFlagHelper mapi, pa vec vracaju
        // null bez ikakve posebne provere) - u tom slucaju se prikazuje ikonica planete.
        public string? FlagImage => Helpers.CountryFlagHelper.GetFlagImage(Country);

        public bool HasFlag => FlagImage is not null;

        public bool NoFlag => !HasFlag;
    }

    public class Squad
    {
        public long PlayerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public string Position { get; set; }

        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);

        public bool NoImage => !HasImage;

        public string PlayerDisplay => (FirstName, LastName) switch
        {
            (var f, var l) when !string.IsNullOrWhiteSpace(f) && !string.IsNullOrWhiteSpace(l) => $"{f} {l}",
            (var f, _) when !string.IsNullOrWhiteSpace(f) => f!,
            (_, var l) when !string.IsNullOrWhiteSpace(l) => l!,
            _ => string.Empty
        };
    }

    public class TeamEquipment
    {
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public string Username { get; set; }
    }

    public class TeamMatches
    {
        public long EventId { get; set; }

        // Mesto u carousel-u (postavlja ga TeamViewModel kad se detalji ucitaju): prvi = najraniji dostupan mec
        // (nema strelice ulevo), poslednji = najkasniji dostupan mec (nema strelice udesno).
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }

        public bool HasPrevious => !IsFirst;
        public bool HasNext => !IsLast;
        public DateTime EventDate { get; set; }

        // Isti razlog kao EventsDTO.LocalEventDate/EventDetailsDTO.LocalEventDate - backend
        // salje "goli" ISO string bez Z/offseta, System.Text.Json ga deserijalizuje kao
        // Unspecified, ne Utc.
        public DateTime LocalEventDate => DateTime.SpecifyKind(EventDate, DateTimeKind.Utc).ToLocalTime();

        public string League { get; set; }
        public string Season { get; set; }
        public string Round { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public string? HomeTeamShortName { get; set; }
        public string? AwayTeamShortName { get; set; }
        public string HomeTeamLogo { get; set; }
        public string AwayTeamLogo { get; set; }
        public string HomeTeamScore { get; set; }
        public string AwayTeamScore { get; set; }
        public string? Status { get; set; }

        public string HomeTeamDisplayName => string.IsNullOrWhiteSpace(HomeTeamShortName) ? HomeTeamName : HomeTeamShortName;

        public string AwayTeamDisplayName => string.IsNullOrWhiteSpace(AwayTeamShortName) ? AwayTeamName : AwayTeamShortName;

        public bool HasRound => !string.IsNullOrWhiteSpace(Round);

        public bool IsLive => Status == "Live";

        // Odigran/u toku - koristi se da CarouselView pri otvaranju stranice pozicionira na
        // POSLEDNJU odigranu (ili trenutno Live) utakmicu, vidi TeamViewModel.
        public bool IsPlayed => Status != "Upcoming";

        // Ispod Round-a u carousel kartici - datum+vreme se ispisuje za SVE mecove (Live,
        // Upcoming i Finished), "Live" oznaka se prikazuje DODATNO samo za Live.
        public string DateTimeText => LocalEventDate.ToString("dd/MM/yyyy HH:mm");
    }
}
