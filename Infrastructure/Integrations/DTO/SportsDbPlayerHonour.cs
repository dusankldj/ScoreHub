using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbPlayerHonour
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("idPlayer")]
        public string IdPlayer { get; set; } = string.Empty;

        [JsonPropertyName("idTeam")]
        public string IdTeam { get; set; } = string.Empty;

        [JsonPropertyName("idLeague")]
        public string? IdLeague { get; set; }

        [JsonPropertyName("idHonour")]
        public string? IdHonour { get; set; }

        [JsonPropertyName("strHonour")]
        public string StrHonour { get; set; } = string.Empty;

        [JsonPropertyName("strHonourLogo")]
        public string? StrHonourLogo { get; set; }

        [JsonPropertyName("strHonourTrophy")]
        public string? StrHonourTrophy { get; set; }

        [JsonPropertyName("strSeason")]
        public string? StrSeason { get; set; }
    }
}
