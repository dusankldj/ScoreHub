namespace Domain.Entities
{
    /// <summary>
    /// Pravila bodovanja prognoza za jedan sport (jedan red po sportu). Menjaju se direktno u bazi (UPDATE) - bodovi
    /// se racunaju pri citanju, pa se promena odmah odrazava na sve prognoze. Vidi Domain.Predictions.PredictionScoring.
    /// </summary>
    public class SportScoringRule : BaseEntity.BaseEntity
    {
        public long SportId { get; set; }

        // Bodovi za tacan rezultat.
        public int ExactPoints { get; set; }

        // Bodovi za "Close": pogodjen pobednik (ili nereseno) i razlika u rezultatu odstupa najvise za
        // CloseMarginTolerance. Ignorise se ako je tolerancija NULL.
        public int ClosePoints { get; set; }

        // Bodovi samo za pogodjenog pobednika.
        public int WinnerPoints { get; set; }

        // Najveca dozvoljena greska u razlici (golova/poena) za Close; 0 = razlika mora tacno; NULL = sport nema Close.
        public int? CloseMarginTolerance { get; set; }

        // Najveci broj koji se moze upisati kao prognoza za JEDAN tim (npr. odbojka = 3 seta); NULL = bez ogranicenja
        // (osim opsteg 0-999). Proverava se i na serveru i u aplikaciji.
        public int? MaxScore { get; set; }

        public Sport Sport { get; set; }
    }
}
