using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Player : SyncedEntity.SyncedEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public long? TeamId { get; set; }
        public long? SecondaryTeamId { get; set; }
        public string ImageUrl { get; set; }
        public DateOnly BirthDate { get; set; }
        public string BirthLocation { get; set; }
        public DateOnly? DateDied { get; set; }
        public string Position { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string Weight { get; set; }
        public string Height { get; set; }
        public string? Worth { get; set; } 
        public string? FacebookLink { get; set; } 
        public string? InstagramLink { get; set; }
        public string? TwitterLink { get; set; } 
        public string? YouTubeLink { get; set; } 
        public DateTime? StatisticsSyncedAt { get; set; }
        public DateTime? FormerTeamsSyncedAt { get; set; }
        public DateTime? HonoursSyncedAt { get; set; }
        public DateTime? MilestonesSyncedAt { get; set; }



        public Team Team { get; set; }
        public Team? SecondaryTeam { get; set; }
        public ICollection<PlayerStatistic> Statistics { get; set; }
        public ICollection<PlayerFormerTeam> FormerTeams { get; set; }
        public ICollection<PlayerHonour> Honours { get; set; }
        public ICollection<PlayerMilestone> Milestones { get; set; }
    }
}
