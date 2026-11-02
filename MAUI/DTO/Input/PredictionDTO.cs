namespace MAUI.DTO.Input
{
    public class PredictionDTO
    {
        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }

        // Ishod i bodovi stizu sa servera (Exact +3 / Winner +1 / Missed / Void / Postponed / Pending).
        public string Outcome { get; set; } = "Pending";
        public int Points { get; set; }
    }
}
