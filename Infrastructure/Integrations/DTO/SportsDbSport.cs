using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbSport
    {
        [JsonPropertyName("idSport")]
        public string IdSport { get; set; } = string.Empty;

        [JsonPropertyName("strSport")]
        public string StrSport { get; set; } = string.Empty;

        [JsonPropertyName("strSportThumb")]
        public string? StrSportThumb { get; set; }

        [JsonPropertyName("strSportDescription")]
        public string? StrSportDescription { get; set; }
    }

    public class SportsDbSportResponse
    {
        [JsonPropertyName("sports")]
        public List<SportsDbSport>? Sports { get; set; }
    }
}
