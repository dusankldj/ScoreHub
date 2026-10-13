namespace WebAPI.DTO.Input
{
    public class RecordSearchDTO
    {
        // "Player" / "Team" / "League" (bez obzira na velika/mala slova).
        public string Kind { get; set; } = string.Empty;
        public long EntityId { get; set; }
    }
}
