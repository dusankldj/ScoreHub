namespace WebAPI.DTO.Output
{
    public class PredictionDTO
    {
        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }

        // "Exact" / "Winner" / "Missed" / "Void" / "Postponed" / "Pending" i osvojeni bodovi (PredictionScoring).
        public string Outcome { get; set; } = "Pending";
        public int Points { get; set; }
    }
}
