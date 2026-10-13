using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbPlayerMilestone
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("idPlayer")]
        public string IdPlayer { get; set; } = string.Empty;

        [JsonPropertyName("idTeam")]
        public string IdTeam { get; set; } = string.Empty;

        [JsonPropertyName("idMilestone")]
        public string? IdMilestone { get; set; }

        [JsonPropertyName("strMilestone")]
        public string StrMilestone { get; set; } = string.Empty;

        [JsonPropertyName("strMilestoneLogo")]
        public string? StrMilestoneLogo { get; set; }

        [JsonPropertyName("dateMilestone")]
        public string? DateMilestone { get; set; }
    }
}
