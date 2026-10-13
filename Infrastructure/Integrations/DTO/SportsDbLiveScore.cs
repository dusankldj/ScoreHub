using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    /// <summary>
    /// Oblik jednog reda iz /livescore/{sport} endpointa. Za razliku od lookup/event/{id},
    /// OVDE strProgress stvarno dolazi popunjen ("18" = 18. minut, "2" = 2. set...) -
    /// lookup/event ga uvek vraca praznog. Koristi se SAMO za dopunjavanje Progress-a,
    /// ostatak podataka o mecu i dalje dolazi iz lookup/event (bogatiji odgovor).
    /// </summary>
    public class SportsDbLiveScore
    {
        [JsonPropertyName("idEvent")]
        public string IdEvent { get; set; } = string.Empty;

        [JsonPropertyName("strStatus")]
        public string? StrStatus { get; set; }

        [JsonPropertyName("strProgress")]
        public string? StrProgress { get; set; }
    }
}
