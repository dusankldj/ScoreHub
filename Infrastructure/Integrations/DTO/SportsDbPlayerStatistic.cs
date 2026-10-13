using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbPlayerStatistic
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("idPlayer")]
        public string IdPlayer { get; set; } = string.Empty;

        [JsonPropertyName("idTeam")]
        public string? IdTeam { get; set; }

        [JsonPropertyName("idLeague")]
        public string? IdLeague { get; set; }

        [JsonPropertyName("strStatistic")]
        public string StrStatistic { get; set; } = string.Empty;

        [JsonPropertyName("strValue")]
        public string StrValue { get; set; } = string.Empty;

        [JsonPropertyName("strSeason")]
        public string? StrSeason { get; set; }
    }
}
