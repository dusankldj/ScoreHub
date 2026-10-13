namespace WebAPI.DTO.Output
{
    public class PlayerSearchResultDTO
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ImageUrl { get; set; }
        public string? SportName { get; set; }
    }

    public class TeamSearchResultDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class LeagueSearchResultDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
    }
}
