using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbTeamEquipment
    {
        [JsonPropertyName("idEquipment")]
        public string IdEquipment { get; set; } = string.Empty;

        [JsonPropertyName("idTeam")]
        public string IdTeam { get; set; } = string.Empty;

        [JsonPropertyName("strSeason")]
        public string? StrSeason { get; set; }

        [JsonPropertyName("strEquipment")]
        public string? StrEquipment { get; set; }

        [JsonPropertyName("strType")]
        public string? StrType { get; set; }

        [JsonPropertyName("strUsername")]
        public string? StrUsername { get; set; }
    }
}
