using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbEventStatistic
    {
        [JsonPropertyName("idStatistic")]
        public string IdStatistic { get; set; } = string.Empty;

        [JsonPropertyName("idEvent")]
        public string IdEvent { get; set; } = string.Empty;

        [JsonPropertyName("idApiFootball")]
        public string? IdApiFootball { get; set; }

        [JsonPropertyName("strEvent")]
        public string? StrEvent { get; set; }

        [JsonPropertyName("strStat")]
        public string StrStat { get; set; } = string.Empty;

        [JsonPropertyName("intHome")]
        public string? IntHome { get; set; }

        [JsonPropertyName("intAway")]
        public string? IntAway { get; set; }
    }
}
