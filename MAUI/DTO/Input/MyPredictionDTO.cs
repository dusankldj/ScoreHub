namespace MAUI.DTO.Input
{
    public class MyPredictionDTO
    {
        public long EventId { get; set; }
        public DateTime EventDate { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }

        // Ishod i bodovi stizu sa servera (PredictionScoring, pravila po sportu): Exact / Close / Winner / Missed / Void (otkazan) /
        // Postponed (odlozen, prognoza i dalje vazi) / Pending.
        public string Outcome { get; set; } = "Pending";
        public int Points { get; set; }

        // ---- Poredjenje sa ulogovanim korisnikom (samo UserPage, filter "Common") ----
        // Mine = MOJA prognoza za isti mec (null ako ga nisam prognozirao); OtherUsername = ime prijatelja ciju
        // listu gledamo; ShowComparison = trenutno je izabran filter "Common" (postavlja PredictionsListModel).
        public MyPredictionDTO? Mine { get; set; }
        public string OtherUsername { get; set; } = string.Empty;
        public bool ShowComparison { get; set; }

        public bool HasComparison => ShowComparison && Mine is not null;

        public bool NoComparison => !HasComparison;

        private static string Format(int home, int away, int points)
            => points > 0 ? $"{home}-{away}  (+{points})" : $"{home}-{away}";

        public string YouDisplay => Mine is null ? string.Empty : $"You  {Format(Mine.PredictedHomeScore, Mine.PredictedAwayScore, Mine.Points)}";

        public string FriendDisplay => $"{OtherUsername}  {Format(PredictedHomeScore, PredictedAwayScore, Points)}";

        private bool IsSettledOutcome => Outcome is "Exact" or "Close" or "Winner" or "Missed";

        // +1 = ja sam osvojio vise bodova, -1 = prijatelj, 0 = izjednaceno ILI mec jos nema ishod (nema sta da se poredi).
        private int Comparison => Mine is null || !IsSettledOutcome ? 0 : Mine.Points.CompareTo(Points);

        // Zeleno = ja sam bolji, narandzasto = prijatelj je bolji, sivo = jednako / nije zavrseno.
        public Color ComparisonAccent => Comparison > 0 ? PredictionColors.Exact : Comparison < 0 ? PredictionColors.Winner : PredictionColors.Neutral;

        public Color ComparisonBackground => Comparison > 0 ? Color.FromArgb("#E8F5E9") : Comparison < 0 ? Color.FromArgb("#FFF3E0") : Color.FromArgb("#F5F5F5");

        // Isto kao EventsDTO - backend salje EventDate bez Z/offseta, pa se eksplicitno "pecatira" kao Utc.
        public DateTime LocalEventDate => DateTime.SpecifyKind(EventDate, DateTimeKind.Utc).ToLocalTime();

        public bool IsFinished => Status == "Finished";

        public bool IsLive => Status == "Live";

        public bool IsUpcoming => Status == "Upcoming";

        // Hit = osvojeni bodovi (tacan rezultat ILI pobednik), Missed = 0 bodova; null dok nema ishoda
        // (nije odigran / traje / odlozen / otkazan - otkazan nije ni pogodak ni promasaj).
        public bool? Hit => Outcome switch
        {
            "Exact" or "Close" or "Winner" => true,
            "Missed" => false,
            _ => null
        };

        public bool IsVoid => Outcome == "Void";

        public bool IsPostponed => Outcome == "Postponed";

        public string MatchTitle => $"{HomeTeam} vs {AwayTeam}";

        public string PredictionDisplay => Points > 0
            ? $"Your prediction: {PredictedHomeScore}-{PredictedAwayScore}  (+{Points})"
            : $"Your prediction: {PredictedHomeScore}-{PredictedAwayScore}";

        // Tacan: zeleno, pobednik: narandzasto, promasaj: crveno; otkazan/ostalo: neutralno.
        public Color AccentColor => HasComparison ? ComparisonAccent : Outcome switch
        {
            "Exact" => PredictionColors.Exact,
            "Close" => PredictionColors.Close,
            "Winner" => PredictionColors.Winner,
            "Missed" => PredictionColors.Missed,
            "Void" => PredictionColors.Neutral,
            _ => Color.FromArgb("#1A1A2E")
        };

        public Color CardBackground => HasComparison ? ComparisonBackground : Outcome switch
        {
            "Exact" => Color.FromArgb("#E8F5E9"),
            "Close" => Color.FromArgb("#E0F2F1"),
            "Winner" => Color.FromArgb("#FFF3E0"),
            "Missed" => Color.FromArgb("#FFEBEE"),
            _ => Color.FromArgb("#F5F5F5")
        };

        // Zavrsen/Live: rezultat meca; Upcoming: vreme pocetka; otkazan/odlozen: oznaka umesto rezultata.
        public string RightTopDisplay => IsVoid ? "Cancelled"
            : IsPostponed ? "Postponed"
            : IsUpcoming ? LocalEventDate.ToString("HH:mm")
            : $"{HomeTeamScore?.ToString() ?? "-"} - {AwayTeamScore?.ToString() ?? "-"}";

        public string RightBottomDisplay => IsLive ? "Live" : LocalEventDate.ToString("dd/MM/yyyy");

        public Color RightBottomColor => IsLive ? PredictionColors.Missed : Color.FromArgb("#8A8A8A");
    }
}
