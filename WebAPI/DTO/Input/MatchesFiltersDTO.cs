namespace WebAPI.DTO.Input
{
    public class MatchesFiltersDTO
    {
        public long? SportId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        // "Live" | "Upcoming" | "Finished" | null (bez filtera) - poredi se sa VEC izracunatim
        // stanjem (MatchesController.ComputeState), ne sa sirovim Match.Status iz baze.
        public string? Status { get; set; }
    }
}
