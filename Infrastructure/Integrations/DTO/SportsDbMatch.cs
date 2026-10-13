using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Integrations.DTO
{
    /// <summary>
    /// Oblik jednog meča iz schedule/next (ili slično) endpointa. SVI id* stringovi
    /// (idLeague, idHomeTeam, idAwayTeam, idVenue) su SportsDB ExternalId vrednosti -
    /// pri mapiranju u Match entitet, svaki od njih se prevodi u naš long FK preko
    /// pretrage postojećeg reda po ExternalId (isti obrazac kao League.SportId/Team.LeagueId).
    /// </summary>
    public class SportsDbMatch
    {
        [JsonPropertyName("idEvent")]
        public string IdEvent { get; set; } = string.Empty;

        [JsonPropertyName("strEvent")]
        public string StrEvent { get; set; } = string.Empty;

        [JsonPropertyName("strSport")]
        public string? StrSport { get; set; }

        [JsonPropertyName("strTimestamp")]
        public string? StrTimestamp { get; set; } // "2026-08-22T16:30:00" - pun datum+vreme, precizniji od dateEvent+strTime posebno

        [JsonPropertyName("dateEvent")]
        public string? DateEvent { get; set; } // "2026-08-22" - rezerva ako strTimestamp fali

        [JsonPropertyName("strTime")]
        public string? StrTime { get; set; } // "16:30:00" - rezerva ako strTimestamp fali

        [JsonPropertyName("idLeague")]
        public string? IdLeague { get; set; }

        [JsonPropertyName("strLeague")]
        public string? StrLeague { get; set; }

        [JsonPropertyName("strSeason")]
        public string? StrSeason { get; set; }

        [JsonPropertyName("intRound")]
        public string? IntRound { get; set; }

        [JsonPropertyName("idHomeTeam")]
        public string? IdHomeTeam { get; set; }

        [JsonPropertyName("strHomeTeam")]
        public string? StrHomeTeam { get; set; }

        [JsonPropertyName("idAwayTeam")]
        public string? IdAwayTeam { get; set; }

        [JsonPropertyName("strAwayTeam")]
        public string? StrAwayTeam { get; set; }

        [JsonPropertyName("intHomeScore")]
        public string? IntHomeScore { get; set; } // null dok se ne odigra

        [JsonPropertyName("intAwayScore")]
        public string? IntAwayScore { get; set; }

        [JsonPropertyName("idVenue")]
        public string? IdVenue { get; set; }

        [JsonPropertyName("strVenue")]
        public string? StrVenue { get; set; }

        [JsonPropertyName("strStatus")]
        public string? StrStatus { get; set; } // "NS" (Not Started), itd.

        [JsonPropertyName("strProgress")]
        public string? StrProgress { get; set; } // npr. "45" (minut), popunjeno samo dok je mec uzivo

        [JsonPropertyName("strPostponed")]
        public string? StrPostponed { get; set; }
    }
}
