namespace WebAPI.DTO.Output
{
    public class MatchesDTO
    {
        public long SportId { get; set; }
        public string SportName { get; set; }

        public List<LeaguesDTO>? Leagues { get; set; } = new List<LeaguesDTO>();
    }

    public class LeaguesDTO
    {
        public long LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string? LeagueLogoUrl { get; set; }

        public List<EventsDTO>? Events { get; set; } = new List<EventsDTO>();
    }

    public class EventsDTO
    {
        public long EventId { get; set; }
        public DateTime EventDate { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeamLogoUrl { get; set; }
        public string AwayTeamLogoUrl { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public string? Status { get; set; } = string.Empty;

        // Sirov provider kod (1H/2H/HT/S1-S4/ADB...) - SAMO dok je Status=="Live", za precizniji
        // prikaz od generickog "Live" u listi meceva (isti izvor kao EventDetailsDTO.DetailedStatus).
        public string? DetailedStatus { get; set; }

        // Minut/set brojac (npr "66", "2") - ide UZ DetailedStatus ("2H - 66'"). Popunjava se iz
        // livescore/{sport} endpointa (vidi SyncLiveMatchStatisticsAsync) - lookup/event ga
        // uvek vraca praznog.
        public string? Progress { get; set; }
    }
}
