using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    /// <summary>lookup/event_timeline/{id} - potvrdjeno u praksi samo za fudbal.</summary>
    public class SportsDbEventTimeline
    {
        [JsonPropertyName("idTimeline")]
        public string IdTimeline { get; set; } = string.Empty;

        [JsonPropertyName("idEvent")]
        public string IdEvent { get; set; } = string.Empty;

        [JsonPropertyName("strTimeline")]
        public string StrTimeline { get; set; } = string.Empty;

        [JsonPropertyName("strTimelineDetail")]
        public string? StrTimelineDetail { get; set; }

        // "Yes" / "No"
        [JsonPropertyName("strHome")]
        public string? StrHome { get; set; }

        [JsonPropertyName("idAPIfootball")]
        public string? IdApiFootball { get; set; }

        [JsonPropertyName("idPlayer")]
        public string? IdPlayer { get; set; }

        [JsonPropertyName("strPlayer")]
        public string? StrPlayer { get; set; }

        [JsonPropertyName("strCutout")]
        public string? StrCutout { get; set; }

        // "0" ili prazno kad nema asistenta
        [JsonPropertyName("idAssist")]
        public string? IdAssist { get; set; }

        [JsonPropertyName("strAssist")]
        public string? StrAssist { get; set; }

        [JsonPropertyName("intTime")]
        public string? IntTime { get; set; }

        [JsonPropertyName("strPeriod")]
        public string? StrPeriod { get; set; }

        [JsonPropertyName("idTeam")]
        public string? IdTeam { get; set; }

        // Provajder salje literalni tekst "NULL" kad nema komentara.
        [JsonPropertyName("strComment")]
        public string? StrComment { get; set; }
    }
}
