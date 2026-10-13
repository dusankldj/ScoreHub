namespace WebAPI.DTO.Output
{
    public class FavoriteTeamDTO
    {
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public string? ShortName { get; set; }
        public string LogoUrl { get; set; }
        public string SportName { get; set; }
    }
}
