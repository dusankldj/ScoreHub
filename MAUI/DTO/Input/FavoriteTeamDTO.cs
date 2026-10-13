namespace MAUI.DTO.Input
{
    public class FavoriteTeamDTO
    {
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public string? ShortName { get; set; }
        public string LogoUrl { get; set; }
        public string SportName { get; set; }

        public bool HasShortName => !string.IsNullOrWhiteSpace(ShortName);

        // "TeamName (ShortName)" - izostavlja zagradu ako ShortName ne postoji.
        public string TeamDisplayName => HasShortName ? $"{TeamName} ({ShortName})" : TeamName;
    }
}
