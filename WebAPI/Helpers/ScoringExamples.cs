namespace WebAPI.Helpers
{
    // Primer prognoze + nekoliko mogucih stvarnih rezultata po sportu za stranicu sa pravilima bodovanja. Ishod i bodovi
    // se ne pisu ovde, nego ih racuna PredictionScoring prema pravilu sporta (tabela SportScoringRules).
    public static class ScoringExamples
    {
        public static (int Home, int Away, (int Home, int Away)[] Results) For(string sportName) => sportName switch
        {
            "Soccer" => (2, 1, new[] { (2, 1), (3, 2), (3, 0), (1, 1) }),
            "Ice Hockey" => (3, 2, new[] { (3, 2), (4, 3), (5, 2), (2, 3) }),
            "Handball" => (30, 28, new[] { (30, 28), (31, 28), (33, 28), (27, 29) }),
            "Basketball" => (98, 92, new[] { (98, 92), (104, 97), (108, 96), (90, 95) }),
            "American Football" => (27, 20, new[] { (27, 20), (24, 20), (31, 17), (17, 24) }),
            "Volleyball" => (3, 1, new[] { (3, 1), (3, 0), (3, 2), (1, 3) }),
            _ => (2, 1, new[] { (2, 1), (3, 2), (3, 0), (1, 2) })
        };
    }
}
