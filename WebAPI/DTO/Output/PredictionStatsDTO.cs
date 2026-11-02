namespace WebAPI.DTO.Output
{
    public class PredictionStatsDTO
    {
        public int TotalPredictions { get; set; }
        public int Settled { get; set; }
        public int Exact { get; set; }
        // Pogodjen pobednik i razlika u okviru tolerancije sporta (vidi SportScoringRule).
        public int Close { get; set; }
        public int Winner { get; set; }
        public int Missed { get; set; }
        // Otkazani/prekinuti mecevi - prognoza ponistena (bez bodova, van statistike).
        public int Voided { get; set; }
        // Jos bez ishoda (nije odigran, traje, odlozen).
        public int Pending { get; set; }
        public int Points { get; set; }
        // (Exact + Winner) / Settled, u procentima.
        public double SuccessRate { get; set; }
        public int CurrentStreak { get; set; }
        public int BestStreak { get; set; }
        // Plasman medju korisnikom i njegovim prijateljima (rang-lista).
        public int? Rank { get; set; }
        public int RankedUsers { get; set; }
        // Poslednjih (do) 10 ishoda, od NAJSTARIJEG ka najnovijem: "E" (tacan), "C" (close), "W" (pobednik), "M" (promasaj).
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
        // Red ulogovanog korisnika (ili null ako jos nije rangiran) - vraca se i kad nije u prikazanom topu.
        public LeaderboardEntryDTO? Me { get; set; }
        public int RankedUsers { get; set; }
    }
}
