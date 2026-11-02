namespace MAUI.DTO.Input
{
    public class PredictionStatsDTO
    {
        public int TotalPredictions { get; set; }
        public int Settled { get; set; }
        public int Exact { get; set; }
        public int Close { get; set; }
        public int Winner { get; set; }
        public int Missed { get; set; }
        public int Voided { get; set; }
        public int Pending { get; set; }
        public int Points { get; set; }
        public double SuccessRate { get; set; }
        public int CurrentStreak { get; set; }
        public int BestStreak { get; set; }
        public int? Rank { get; set; }
        public int RankedUsers { get; set; }

        // "E" (tacan rezultat), "C" (close), "W" (pobednik), "M" (promasaj) - od najstarijeg ka najnovijem.
        public List<string> RecentForm { get; set; } = new();
    }

    public class LeaderboardEntryDTO
    {
        public long UserId { get; set; }
        public int Rank { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Points { get; set; }
        public int Exact { get; set; }
        public int Settled { get; set; }
        public bool IsMe { get; set; }
    }

    public class LeaderboardDTO
    {
        public List<LeaderboardEntryDTO> Entries { get; set; } = new();
        public LeaderboardEntryDTO? Me { get; set; }
        public int RankedUsers { get; set; }
    }

    // Boje ishoda prognoze na jednom mestu - isto se koristi u listi prognoza, grafikonu i "formi".
    public static class PredictionColors
    {
        public static readonly Color Exact = Color.FromArgb("#2E7D32");
        public static readonly Color Close = Color.FromArgb("#00897B");
        public static readonly Color Winner = Color.FromArgb("#EF6C00");
        public static readonly Color Missed = Color.FromArgb("#D32F2F");
        public static readonly Color Neutral = Color.FromArgb("#8A8A8A");
    }

    // Jedan kruzic "forme" (poslednjih 10 ishoda).
    public class FormDotDTO
    {
        public string Letter { get; set; } = string.Empty;
        public Color Color { get; set; } = PredictionColors.Neutral;
    }

    // Jedan red rang-liste: traka (ProgressBar) je srazmerna bodovima prvoplasiranog.
    public class LeaderboardRowDTO
    {
        public long UserId { get; set; }
        public int Rank { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Points { get; set; }
        public int Exact { get; set; }
        public int Settled { get; set; }
        public bool IsMe { get; set; }
        public double Ratio { get; set; }

        public string RankDisplay => $"{Rank}.";
        public string PointsDisplay => $"{Points} pts";
        public string Detail => $"{Exact} exact / {Settled} played";
        public Color BarColor => IsMe ? PredictionColors.Exact : Color.FromArgb("#9AA5B1");
        public Color RowBackground => IsMe ? Color.FromArgb("#E8F5E9") : Colors.Transparent;
        public FontAttributes NameAttributes => IsMe ? FontAttributes.Bold : FontAttributes.None;
    }
}
