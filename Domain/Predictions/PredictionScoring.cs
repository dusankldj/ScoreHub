namespace Domain.Predictions
{
    public enum PredictionOutcome
    {
        // Mec jos nema konacan ishod (nije poceo / traje / nema rezultat) - ne racuna se nigde.
        Pending,
        // Odlozen (PST/POST) - prognoza OSTAJE VAZECA i ceka novi termin (kad provajder pomeri datum, mec je opet Upcoming).
        Postponed,
        // Otkazan/prekinut (CANC/ABD) - prognoza se PONISTAVA: bez bodova, ni pogodak ni promasaj.
        Void,
        Missed,
        Winner,
        // Pogodjen pobednik (ili nereseno) I razlika u rezultatu odstupa najvise za toleranciju sporta.
        Close,
        Exact
    }

    // Pravila bodovanja za JEDAN sport (red iz tabele SportScoringRules). CloseMarginTolerance == null znaci da nivo
    // "Close" za taj sport ne postoji (npr. odbojka); 0 = razlika mora da se poklopi tacno.
    public sealed record ScoringRule(int ExactPoints, int ClosePoints, int WinnerPoints, int? CloseMarginTolerance, int? MaxScore = null)
    {
        // Rezervno pravilo za sport bez reda u tabeli: tacan rezultat 3, pobednik 1, bez nivoa Close.
        public static readonly ScoringRule Default = new(3, 0, 1, null);

        // Kratko objasnjenje nivoa "Close" za ovaj sport (null ako ga sport nema) - prikazuje se na stranici sa pravilima.
        public string? CloseDescription => CloseMarginTolerance switch
        {
            null => null,
            0 => "Correct winner and the exact score difference",
            1 => "Correct winner and the score difference off by at most 1",
            var tolerance => $"Correct winner and the score difference off by at most {tolerance}"
        };
    }

    // Pravila svih sportova, ucitana jednom (tabela je mala). Sport bez reda dobija ScoringRule.Default.
    public sealed class ScoringRuleSet
    {
        private readonly IReadOnlyDictionary<long, ScoringRule> _rules;

        public ScoringRuleSet(IReadOnlyDictionary<long, ScoringRule> rules)
        {
            _rules = rules;
        }

        public ScoringRule For(long sportId) => _rules.TryGetValue(sportId, out var rule) ? rule : ScoringRule.Default;
    }

    // Jedino mesto gde se odredjuje ishod prognoze i bodovi - koriste ga lista prognoza, statistika, lideri i
    // push obavestenje, pa sva cetiri uvek daju isti rezultat. Bodovi se racunaju pri citanju iz rezultata
    // meca i pravila sporta (ScoringRule); kolona Prediction.Points se namerno ne koristi.
    public static class PredictionScoring
    {
        // Statusi sa KONACNIM rezultatom (iz stvarnih podataka: FT/AET/PEN/AOT/AP + dosudjeni AWD/AW/WO).
        // Whitelist je namerno stroga: mec sa rezultatom ali statusom "NS"/"PST"/"2H"... se NE racuna.
        public static readonly string[] SettledStatusCodes = { "FT", "AET", "PEN", "AOT", "AP", "AWD", "AW", "WO" };

        private static readonly HashSet<string> SettledStatuses = new(SettledStatusCodes, StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> VoidStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "CANC", "ABD", "ADB"
        };

        private static readonly HashSet<string> PostponedStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "PST", "POST"
        };

        public static bool IsVoid(string? status) => status is not null && VoidStatuses.Contains(status);

        public static bool IsPostponed(string? status) => status is not null && PostponedStatuses.Contains(status);

        // Redosled nivoa: Exact (tacan rezultat) > Close (pobednik + razlika u okviru tolerancije) > Winner (pobednik) > Missed.
        // Primenjuje se NAJVISI pogodjeni nivo (bodovi se ne sabiraju).
        public static PredictionOutcome Evaluate(string? status, int? homeScore, int? awayScore, int predictedHome, int predictedAway, ScoringRule rule)
        {
            if (IsVoid(status))
                return PredictionOutcome.Void;

            if (IsPostponed(status))
                return PredictionOutcome.Postponed;

            if (status is null || !SettledStatuses.Contains(status) || homeScore is not int home || awayScore is not int away)
                return PredictionOutcome.Pending;

            if (predictedHome == home && predictedAway == away)
                return PredictionOutcome.Exact;

            var predictedMargin = predictedHome - predictedAway;
            var actualMargin = home - away;

            if (Math.Sign(predictedMargin) != Math.Sign(actualMargin))
                return PredictionOutcome.Missed;

            if (rule.CloseMarginTolerance is int tolerance && Math.Abs(predictedMargin - actualMargin) <= tolerance)
                return PredictionOutcome.Close;

            return PredictionOutcome.Winner;
        }

        public static int Points(PredictionOutcome outcome, ScoringRule rule) => outcome switch
        {
            PredictionOutcome.Exact => rule.ExactPoints,
            PredictionOutcome.Close => rule.ClosePoints,
            PredictionOutcome.Winner => rule.WinnerPoints,
            _ => 0
        };

        // Ishod koji ulazi u statistiku i rang-listu (Exact/Close/Winner/Missed) - Pending/Postponed/Void ne ulaze.
        public static bool IsSettled(PredictionOutcome outcome)
            => outcome is PredictionOutcome.Exact or PredictionOutcome.Close or PredictionOutcome.Winner or PredictionOutcome.Missed;
    }
}
