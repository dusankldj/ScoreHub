namespace WebAPI.DTO.Output
{
    public class PlayerDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public string? BirthLocation { get; set; }
        public string? Team { get; set; }
        public string? SecondaryTeam { get; set; }
        public string? Position { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Weight { get; set; }
        public string? Height { get; set; }
        public string? Worth { get; set; }
        public string? FacebookLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? YoutubeLink { get; set; }
        public List<PlayerStatistics> Statistics { get; set; } = new List<PlayerStatistics>();
        public List<PlayerHonours> Honours { get; set; } = new List<PlayerHonours>();
        public List<PlayerMilestones> Milestones { get; set; } = new List<PlayerMilestones>();
        public List<PlayerFormerTeams> FormerTeams { get; set; } = new List<PlayerFormerTeams>();
    }

    public class PlayerStatistics
    {
        public string? Team { get; set; }
        public string? League { get; set; }
        public string Statistic { get; set; }
        public string Value { get; set; }
        public string Season { get; set; }
    }

    public class PlayerHonours
    {
        public string Team { get; set; }
        public string League { get; set; }
        public string HonourName { get; set; }
        public string HonourTrophyUrl { get; set; }
        public string Season { get; set; }
    }

    public class PlayerMilestones
    {
        public string Team { get; set; }
        public string MilestoneName { get; set; }
        public string? MilestoneUrl { get; set; }
        public DateOnly DateAchieved { get; set; }
    }

    public class PlayerFormerTeams
    {
        public string Team { get; set; }
        public string TeamLogoUrl { get; set; }
        public string MoveType { get; set; }
        public string Joined { get; set; }
        public string? Departed { get; set; }
        public string? Appearences { get; set; }
        public string? Goals { get; set; }
    }
}
