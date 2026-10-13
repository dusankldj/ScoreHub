using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    /// <summary>
    /// search/venue/{ime} vraća idVenue kao BROJ, ne string (za razliku od skoro svih
    /// ostalih ID-jeva u ovom API-ju) - zato je tip long, ne string kao na drugim DTO-ovima.
    /// </summary>
    public class SportsDbVenue
    {
        [JsonPropertyName("idVenue")]
        public long IdVenue { get; set; }

        [JsonPropertyName("strVenue")]
        public string StrVenue { get; set; } = string.Empty;

        [JsonPropertyName("strLocation")]
        public string? StrLocation { get; set; }

        [JsonPropertyName("strCountry")]
        public string? StrCountry { get; set; }

        [JsonPropertyName("strThumb")]
        public string? StrThumb { get; set; }

        [JsonPropertyName("strSport")]
        public string? StrSport { get; set; }
    }
}
