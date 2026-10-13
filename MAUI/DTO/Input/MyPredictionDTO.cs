namespace MAUI.DTO.Input
{
    public class MyPredictionDTO
    {
        private static readonly Color HitColor = Color.FromArgb("#2E7D32");
        private static readonly Color MissColor = Color.FromArgb("#D32F2F");
        private static readonly Color NeutralColor = Color.FromArgb("#1A1A2E");

        public long EventId { get; set; }
        public DateTime EventDate { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }

        // Isto kao EventsDTO - backend salje EventDate bez Z/offseta, pa se eksplicitno "pecatira" kao Utc.
        public DateTime LocalEventDate => DateTime.SpecifyKind(EventDate, DateTimeKind.Utc).ToLocalTime();

        public bool IsFinished => Status == "Finished";

        public bool IsLive => Status == "Live";

        public bool IsUpcoming => Status == "Upcoming";

        // Pogodak/promasaj se odredjuje SAMO za zavrsen mec sa poznatim rezultatom; inace null (jos nema ishoda).
        public bool? Hit => IsFinished && HomeTeamScore is int home && AwayTeamScore is int away
            ? PredictedHomeScore == home && PredictedAwayScore == away
            : null;

        public string MatchTitle => $"{HomeTeam} vs {AwayTeam}";

        public string PredictionDisplay => $"Your prediction: {PredictedHomeScore}-{PredictedAwayScore}";

        // Zavrsen: zeleno (pogodak) / crveno (promasaj); Live/Upcoming: neutralno.
        public Color AccentColor => Hit switch
        {
            true => HitColor,
            false => MissColor,
            _ => NeutralColor
        };

        public Color CardBackground => Hit switch
        {
            true => Color.FromArgb("#E8F5E9"),
            false => Color.FromArgb("#FFEBEE"),
            _ => Color.FromArgb("#F5F5F5")
        };

        // Zavrsen/Live: rezultat meca; Upcoming: vreme pocetka.
        public string RightTopDisplay => IsUpcoming
            ? LocalEventDate.ToString("HH:mm")
            : $"{HomeTeamScore?.ToString() ?? "-"} - {AwayTeamScore?.ToString() ?? "-"}";

        public string RightBottomDisplay => IsLive ? "Live" : LocalEventDate.ToString("dd/MM/yyyy");

        public Color RightBottomColor => IsLive ? MissColor : Color.FromArgb("#8A8A8A");
    }
}
