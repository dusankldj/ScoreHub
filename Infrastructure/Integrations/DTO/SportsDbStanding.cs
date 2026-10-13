using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbStanding
    {
        [JsonPropertyName("idStanding")]
        public string IdStanding { get; set; } = string.Empty;

        [JsonPropertyName("intRank")]
        public string? IntRank { get; set; }

        [JsonPropertyName("idTeam")]
        public string IdTeam { get; set; } = string.Empty;

        [JsonPropertyName("strTeam")]
        public string? StrTeam { get; set; }

        [JsonPropertyName("idLeague")]
        public string IdLeague { get; set; } = string.Empty;

        [JsonPropertyName("strSeason")]
        public string? StrSeason { get; set; }

        [JsonPropertyName("strGroup")]
        public string? StrGroup { get; set; }

        [JsonPropertyName("strForm")]
        public string? StrForm { get; set; }

        [JsonPropertyName("strDescription")]
        public string? StrDescription { get; set; }

        [JsonPropertyName("intPlayed")]
        public string? IntPlayed { get; set; }

        [JsonPropertyName("intWin")]
        public string? IntWin { get; set; }

        [JsonPropertyName("intLoss")]
        public string? IntLoss { get; set; }

        [JsonPropertyName("intDraw")]
        public string? IntDraw { get; set; }

        [JsonPropertyName("intGoalsFor")]
        public string? IntGoalsFor { get; set; }

        [JsonPropertyName("intGoalsAgainst")]
        public string? IntGoalsAgainst { get; set; }

        [JsonPropertyName("intGoalDifference")]
        public string? IntGoalDifference { get; set; }

        [JsonPropertyName("intPoints")]
        public string? IntPoints { get; set; }
    }
}
