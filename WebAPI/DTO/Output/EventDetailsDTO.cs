namespace WebAPI.DTO.Output
{
    public class EventDetailsDTO
    {
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public long HomeTeamId { get; set; }
        public long AwayTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public string HomeTeamBadge { get; set; }
        public string AwayTeamBadge { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public string? Progress { get; set; } = string.Empty;
        public string Venue { get; set; }
        public string? Status { get; set; } = string.Empty;

        public string? DetailedStatus { get; set; } = string.Empty;
        public string Season { get; set; } = string.Empty;
        public string LeagueName { get; set; } = string.Empty;
        public string? Round { get; set; }
        public List<EventLineupDTO>? Players { get; set; } = new List<EventLineupDTO>();
        public List<EventStatisticsDTO>? Statistics { get; set; } = new List<EventStatisticsDTO>();
        public List<EventBroadcastDTO>? Broadcasts { get; set; } = new List<EventBroadcastDTO>();
        public List<EventTimelineDTO>? Timeline { get; set; } = new List<EventTimelineDTO>();

        // NEfiltrirano - svaki red iz EventTimelines za ovaj mec (golovi, kartoni, izmene, VAR...),
        // za "Full timeline" karticu. Timeline (iznad) ostaje filtriran samo na golove/kartone.
        public List<EventTimelineDTO>? FullTimeline { get; set; } = new List<EventTimelineDTO>();
    }

    // Samo "bitni" dogadjaji za prikaz (golovi i kartoni), sortirani po minutu - izmene i ostalo se
    // cuvaju u bazi ali se ovde ne salju.
    public class EventTimelineDTO
    {
        public int? Minute { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Detail { get; set; }
        public bool Home { get; set; }
        public string? PlayerName { get; set; }
        public string? AssistName { get; set; }
        public string? Comment { get; set; }
    }

    public class EventBroadcastDTO
    {
        public string ChannelName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Country { get; set; }
    }

    public class EventLineupDTO
    {
        public long PlayerId { get; set; }
        public long TeamId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? PositionShort { get; set; }
        public string? Formation { get; set; }
        public bool HomeGame { get; set; }
        public bool IsSubtitute { get; set; }
        public int SquadNumber { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class EventStatisticsDTO
    {
        public string Stat { get; set; }
        public float HomeScore { get; set; }
        public float AwayScore { get; set; }
    }
}
