using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbEventLineup
    {
        [JsonPropertyName("idLineup")]
        public string IdLineup { get; set; } = string.Empty;

        [JsonPropertyName("idEvent")]
        public string IdEvent { get; set; } = string.Empty;

        [JsonPropertyName("idApiFootball")]
        public string? IdApiFootball { get; set; }

        [JsonPropertyName("strPosition")]
        public string? StrPosition { get; set; }

        [JsonPropertyName("strPositionShort")]
        public string? StrPositionShort { get; set; }

        [JsonPropertyName("strFormation")]
        public string? StrFormation { get; set; }

        [JsonPropertyName("strHome")]
        public string? StrHome { get; set; } // "Yes"/"No"

        [JsonPropertyName("strSubstitute")]
        public string? StrSubstitute { get; set; } // "Yes"/"No"

        [JsonPropertyName("intSquadNumber")]
        public string? IntSquadNumber { get; set; }

        [JsonPropertyName("idPlayer")]
        public string IdPlayer { get; set; } = string.Empty;

        [JsonPropertyName("strPlayer")]
        public string? StrPlayer { get; set; }

        [JsonPropertyName("idTeam")]
        public string IdTeam { get; set; } = string.Empty;

        [JsonPropertyName("strSeason")]
        public string? StrSeason { get; set; }
    }
}
