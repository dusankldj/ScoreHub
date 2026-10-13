namespace MAUI.DTO.Input
{
    public class EventDetailsDTO
    {
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }

        // Isti razlog kao EventsDTO.LocalEventDate - backend salje "goli" ISO string bez
        // Z/offseta, System.Text.Json ga deserijalizuje kao Unspecified, ne Utc.
        public DateTime LocalEventDate => DateTime.SpecifyKind(EventDate, DateTimeKind.Utc).ToLocalTime();

        public long HomeTeamId { get; set; }
        public long AwayTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public string HomeTeamBadge { get; set; }
        public string AwayTeamBadge { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public string? Progress { get; set; }
        public string Venue { get; set; }
        public string? Status { get; set; }
        public string? DetailedStatus { get; set; }
        public string Season { get; set; }
        public string LeagueName { get; set; }
        public string? Round { get; set; }

        public List<EventLineupDTO> Players { get; set; } = new();
        public List<EventStatisticsDTO> Statistics { get; set; } = new();
        public List<EventBroadcastDTO> Broadcasts { get; set; } = new();
        public List<EventTimelineDTO> Timeline { get; set; } = new();
        public List<EventTimelineDTO> FullTimeline { get; set; } = new();
    }

    // Jedan gol/karton u "Timeline" sekciji. Home odredjuje na kojoj se strani prikazuje (levo domacin,
    // desno gost), minut je u sredini uz svaki unos.
    public class EventTimelineDTO
    {
        private static readonly Color YellowCardColor = Color.FromArgb("#F9A825");
        private static readonly Color RedCardColor = Color.FromArgb("#D32F2F");

        public int? Minute { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public bool Home { get; set; }
        public string? PlayerName { get; set; }
        public string? AssistName { get; set; }
        public string? Comment { get; set; }

        public bool IsAway => !Home;

        public bool HasComment => !string.IsNullOrWhiteSpace(Comment);

        // "(Argument)" uz kartonu. Komentar je tekst sa spoljasnjeg provajdera - zato se novi redovi svode na
        // razmak i duzina ogranicava (CommentMaxLength), da neocekivano dug/visered sadrzaj ne razvuce red.
        private const int CommentMaxLength = 40;

        public string CommentDisplay
        {
            get
            {
                if (!HasComment)
                    return string.Empty;

                var text = Comment!.Replace('\r', ' ').Replace('\n', ' ').Trim();

                if (text.Length > CommentMaxLength)
                    text = text[..CommentMaxLength].TrimEnd() + "…";

                return $"({text})";
            }
        }

        public bool IsGoal => string.Equals(Type, "Goal", StringComparison.OrdinalIgnoreCase);

        public bool IsCard => string.Equals(Type, "Card", StringComparison.OrdinalIgnoreCase);

        // "subst" (kod nas) - provajder ponekad salje i "Substitution" (video u praksi kao redak varijantni
        // naziv istog dogadjaja). EMPIRIJSKI potvrdjeno (unakrsnom proverom: igrac koji je izasao ne moze
        // imati KASNIJI gol/karton u istom mecu) da PlayerId ODLAZI, a AssistantId ULAZI - iako se "idAssist"
        // kod gola odnosi na asistenta, provajder ga za izmene koristi za igraca koji ulazi.
        public bool IsSubstitution => string.Equals(Type, "subst", StringComparison.OrdinalIgnoreCase)
            || Type.Contains("Substitution", StringComparison.OrdinalIgnoreCase);

        public bool IsVar => string.Equals(Type, "Var", StringComparison.OrdinalIgnoreCase);

        // Sve sto nije gol/karton/izmena/VAR (npr. "2 Pointer"/"3 Pointer" kod kosarke, "Miss (Pen
        // Shootout)"...) - "Full timeline" ne sme nista da izostavi, pa ovo hvata ostatak.
        public bool IsOther => !IsGoal && !IsCard && !IsSubstitution && !IsVar;

        // Igrac koji ULAZI (zelena strelica gore) / IZLAZI (crvena strelica dole) - vidi IsSubstitution.
        public string? PlayerInName => AssistName;

        public string? PlayerOutName => PlayerName;

        public string VarDisplay => string.IsNullOrWhiteSpace(Detail) ? "VAR" : Detail!;

        // "PlayerName - Type" ili "PlayerName - Type (Detail)" - generican prikaz za IsOther slucaj.
        public string OtherDisplay
        {
            get
            {
                var text = string.IsNullOrWhiteSpace(Detail) ? Type : $"{Type} ({Detail})";
                return string.IsNullOrWhiteSpace(PlayerName) ? text : $"{PlayerName} - {text}";
            }
        }

        public bool HasAssist => IsGoal && !string.IsNullOrWhiteSpace(AssistName);

        public string MinuteDisplay => Minute is int minute ? $"{minute}'" : string.Empty;

        public bool IsPenalty => IsGoal && Detail is not null && Detail.Contains("Penalty", StringComparison.OrdinalIgnoreCase);

        // "(P)" ODMAH uz ikonicu lopte (zaseban Span, pre imena igraca) - penal je svojstvo TIPA gola,
        // ne igraca, za razliku od autogola (koji opisuje KO ga je postigao, ostaje uz ime).
        public string PenaltyMarker => IsPenalty ? " (P)" : string.Empty;

        // Delovi " (<ikonica> Asistent)" kao ZASEBNI Span-ovi (razlicit font za ikonicu) - prazni kad nema asistenta.
        public string AssistOpen => HasAssist ? " (" : string.Empty;

        public string AssistIconText => HasAssist ? Helpers.Icons.AssistIcon : string.Empty;

        public string AssistDisplay => HasAssist ? $" {AssistName})" : string.Empty;

        // "Player (OG)" - autogol ostaje vidljiv uz ime, penal se prikazuje odvojeno uz ikonicu (PenaltyMarker).
        public string GoalPlayerDisplay => Detail is not null && Detail.Contains("Own", StringComparison.OrdinalIgnoreCase)
            ? $"{PlayerName} (OG)"
            : PlayerName ?? string.Empty;

        // Crveni (i drugi zuti, koji je efektivno iskljucenje) -> crveno, inace zuto.
        public Color CardColor => Detail is not null
            && (Detail.Contains("Red", StringComparison.OrdinalIgnoreCase) || Detail.Contains("Second", StringComparison.OrdinalIgnoreCase))
                ? RedCardColor
                : YellowCardColor;

        // Tekst uz sredinu: domacin poravnat udesno (uz minut), gost ulevo.
        public TextAlignment TextAlignment => Home ? TextAlignment.End : TextAlignment.Start;

        // Isto poravnanje za FlexLayout (karton + komentar koji se lomi u novi red).
        public Microsoft.Maui.Layouts.FlexJustify FlexJustify => Home
            ? Microsoft.Maui.Layouts.FlexJustify.End
            : Microsoft.Maui.Layouts.FlexJustify.Start;
    }

    public class EventBroadcastDTO
    {
        public string ChannelName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Country { get; set; }

        public bool HasLogo => !string.IsNullOrWhiteSpace(LogoUrl);

        public bool HasName => !string.IsNullOrWhiteSpace(ChannelName);

        // Originalni logotipi kanala su 800x450 (~1.4 MB dekodirano po slici, a u kartici ih moze biti
        // i 80+) - to je bio uzrok "kocenja" pri skrolovanju. CDN nudi "/small" varijantu (355x200),
        // dovoljnu za logo visine 48dp; proverena je na uzorku logotipa. Nepoznat host -> original.
        public string? LogoThumbUrl => HasLogo && LogoUrl!.Contains("thesportsdb.com/images/") && !LogoUrl.EndsWith("/small")
            ? LogoUrl + "/small"
            : LogoUrl;

        // Ime slike zastavice ("flag_rs") ili null - za nepoznatu zemlju, "World" ili prazno
        // CountryFlagHelper vraca null, pa se umesto zastave prikazuje ikonica planete.
        public string? FlagImage => Helpers.CountryFlagHelper.GetFlagImage(Country);

        public bool HasFlag => FlagImage is not null;

        public bool NoFlag => !HasFlag;
    }

    // Dva kanala po redu u "Broadcasts" kartici (Right je null kad je ukupan broj neparan).
    public class EventBroadcastRowDTO
    {
        public EventBroadcastDTO Left { get; set; } = new();
        public EventBroadcastDTO? Right { get; set; }

        public bool HasRight => Right is not null;
    }

    public class EventLineupDTO
    {
        public long PlayerId { get; set; }
        public long TeamId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? PositionShort { get; set; }
        public string? Formation { get; set; }
        public bool HomeGame { get; set; }
        public bool IsSubtitute { get; set; }
        public int SquadNumber { get; set; }
        public string? ImageUrl { get; set; }

        public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);

        public bool NoImage => !HasImage;

        // "LastName (PositionShort)" - preskace zagradu sa pozicijom ako PositionShort uopste
        // nije poznat.
        public string PlayerDisplay => string.IsNullOrWhiteSpace(PositionShort)
            ? LastName ?? string.Empty
            : $"{LastName} ({PositionShort})";
    }

    public class EventStatisticsDTO
    {
        public string Stat { get; set; }
        public float HomeScore { get; set; }
        public float AwayScore { get; set; }
    }
}
