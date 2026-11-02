namespace MAUI.DTO.Input
{
    // Pravila bodovanja jednog sporta + primeri (sve izracunato na serveru iz tabele SportScoringRules).
    public class SportScoringDTO
    {
        public long SportId { get; set; }
        public string SportName { get; set; } = string.Empty;
        public int ExactPoints { get; set; }
        public int ClosePoints { get; set; }
        public int WinnerPoints { get; set; }
        public string? CloseDescription { get; set; }
        public int? MaxScore { get; set; }
        public string ExamplePrediction { get; set; } = string.Empty;
        public List<ScoringExampleDTO> Examples { get; set; } = new();

        // Postavlja ViewModel: sport meca iz kog je korisnik dosao (istaknut okvir, prvi na listi).
        public bool IsCurrent { get; set; }

        public bool HasClose => !string.IsNullOrWhiteSpace(CloseDescription);

        public string ExactLine => $"Exact score: {Pts(ExactPoints)}";

        public string CloseLine => HasClose ? $"{CloseDescription}: {Pts(ClosePoints)}" : string.Empty;

        public string WinnerLine => $"Correct winner: {Pts(WinnerPoints)}";

        public bool HasMaxScore => MaxScore is not null;

        public string MaxScoreLine => $"Only scores from 0 to {MaxScore} can be predicted in this sport.";

        public string ExampleHeader => $"Example - you predict {ExamplePrediction}:";

        public Color CardBorder => IsCurrent ? Color.FromArgb("#1A1A2E") : Color.FromArgb("#E0E0E0");

        public double CardBorderWidth => IsCurrent ? 2 : 1;

        private static string Pts(int n) => n == 1 ? "1 point" : $"{n} points";
    }

    public class ScoringExampleDTO
    {
        public string Result { get; set; } = string.Empty;
        public string Outcome { get; set; } = string.Empty;
        public int Points { get; set; }

        public string ResultDisplay => $"Result {Result}";

        public string OutcomeDisplay => Outcome == "Missed"
            ? "Miss  0 pts"
            : $"{Outcome}  +{Points} {(Points == 1 ? "pt" : "pts")}";

        public Color OutcomeColor => Outcome switch
        {
            "Exact" => PredictionColors.Exact,
            "Close" => PredictionColors.Close,
            "Winner" => PredictionColors.Winner,
            _ => PredictionColors.Missed
        };
    }
}
