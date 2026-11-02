namespace WebAPI.DTO.Output
{
    // Pravila bodovanja jednog sporta za stranicu "How points are scored" - iz tabele SportScoringRules, sa primerima
    // izracunatim ISTIM kodom (PredictionScoring) kojim se boduju prave prognoze, pa se nikad ne razilaze sa stvarnoscu.
    public class SportScoringDTO
    {
        public long SportId { get; set; }
        public string SportName { get; set; } = string.Empty;
        public int ExactPoints { get; set; }
        public int ClosePoints { get; set; }
        public int WinnerPoints { get; set; }

        // null = sport nema nivo "Close".
        public string? CloseDescription { get; set; }

        // Najveci broj koji se moze tipovati po timu (npr. odbojka 3); null = bez ogranicenja.
        public int? MaxScore { get; set; }

        public string ExamplePrediction { get; set; } = string.Empty;
        public List<ScoringExampleDTO> Examples { get; set; } = new();
    }

    public class ScoringExampleDTO
    {
        public string Result { get; set; } = string.Empty;

        // "Exact" / "Close" / "Winner" / "Missed"
        public string Outcome { get; set; } = string.Empty;
        public int Points { get; set; }
    }
}
