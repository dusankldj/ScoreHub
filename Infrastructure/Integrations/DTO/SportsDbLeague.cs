using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbLeague
    {
        [JsonPropertyName("idLeague")]
        public string IdLeague { get; set; } = string.Empty;

        [JsonPropertyName("strLeague")]
        public string StrLeague { get; set; } = string.Empty;

        [JsonPropertyName("strSport")]
        public string? StrSport { get; set; }

        [JsonPropertyName("strCountry")]
        public string? StrCountry { get; set; }

        [JsonPropertyName("strBadge")]
        public string? StrBadge { get; set; }

        [JsonPropertyName("strCurrentSeason")]
        public string? StrCurrentSeason { get; set; }
    }

    public class SportsDbLeagueResponse
    {
        [JsonPropertyName("leagues")]
        public List<SportsDbLeague>? Leagues { get; set; }
    }
}

