namespace WebAPI.DTO.Output
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

        // "Exact" / "Winner" / "Missed" / "Void" (otkazan) / "Postponed" / "Pending" - vidi PredictionScoring.
        public string Outcome { get; set; } = "Pending";
        public int Points { get; set; }
    }
}
