namespace WebAPI.DTO.Output
{
    public class LeagueDetailsDTO
    {
        public string Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Country { get; set; }
        public List<LeagueTeamDTO> Teams { get; set; } = new();
        public List<LeagueStandingDTO> Standings { get; set; } = new();
    }

    public class LeagueTeamDTO
    {
        public long TeamId { get; set; }
        public string LogoUrl { get; set; }
        public string TeamName { get; set; }
        public string? ShortName { get; set; }
    }

    public class LeagueStandingDTO
    {
        public int Rank { get; set; }
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public string? TeamShortName { get; set; }
        public string TeamLogoUrl { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesDraw { get; set; }
        public int MatchesLost { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }
        public string? Description { get; set; }
        public string? Form { get; set; }
    }
}
