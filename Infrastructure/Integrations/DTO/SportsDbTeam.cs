using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbTeam
    {
        [JsonPropertyName("idTeam")]
        public string IdTeam { get; set; } = string.Empty;

        [JsonPropertyName("strTeam")]
        public string strTeam { get; set; } = string.Empty;

        [JsonPropertyName("strTeamShort")]
        public string strTeamShort { get; set; } = string.Empty;

        [JsonPropertyName("strSport")]
        public string? StrSport { get; set; }

        [JsonPropertyName("strCountry")]
        public string? StrCountry { get; set; }

        [JsonPropertyName("strBadge")]
        public string? StrBadge { get; set; }

        [JsonPropertyName("strStadium")]
        public string? StrStadium { get; set; }

        [JsonPropertyName("idLeague")]
        public string? IdLeague { get; set; }
    }

    public class SportsDbTeamsResponse
    {
        [JsonPropertyName("teams")]
        public List<SportsDbTeam>? Teams { get; set; }
    }
}
