using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    public class SportsDbEventTv
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("idEvent")]
        public string IdEvent { get; set; } = string.Empty;

        [JsonPropertyName("idChannel")]
        public string IdChannel { get; set; } = string.Empty;

        [JsonPropertyName("strChannel")]
        public string StrChannel { get; set; } = string.Empty;

        [JsonPropertyName("strLogo")]
        public string? StrLogo { get; set; }

        // Zemlja u kojoj kanal emituje (npr. "Brazil"), NE zemlja u kojoj se mec igra (strEventCountry).
        [JsonPropertyName("strCountry")]
        public string? StrCountry { get; set; }

        // Pazi na veliko "S" - JSON imena su case-sensitive. Vreme emitovanja (UTC), npr. "2026-09-08 19:00:00".
        [JsonPropertyName("strTimeStamp")]
        public string? StrTimeStamp { get; set; }
    }
}
