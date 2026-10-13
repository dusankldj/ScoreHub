namespace MAUI.DTO.Input
{
    public class RecentSearchDTO
    {
        public long Id { get; set; }
        public string Kind { get; set; } = string.Empty;
        public long EntityId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? SportName { get; set; }
    }
}
