using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbPlayerFormerTeam
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("idPlayer")]
        public string IdPlayer { get; set; } = string.Empty;

        [JsonPropertyName("idFormerTeam")]
        public string IdFormerTeam { get; set; } = string.Empty;

        [JsonPropertyName("strMoveType")]
        public string? StrMoveType { get; set; }

        [JsonPropertyName("strBadge")]
        public string? StrBadge { get; set; }

        [JsonPropertyName("strJoined")]
        public string? StrJoined { get; set; }

        [JsonPropertyName("strDeparted")]
        public string? StrDeparted { get; set; }

        [JsonPropertyName("intAppearances")]
        public string? IntAppearances { get; set; }

        [JsonPropertyName("intGoals")]
        public string? IntGoals { get; set; }
    }
}
