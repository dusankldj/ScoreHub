namespace Domain.Entities
{
    /// <summary>
    /// Milestone-i (bitni trenuci) u karijeri igrača (lookup/player_milestones/{id}).
    /// MilestoneId (idMilestone) je isto informativna sekundarna oznaka, isti razlog kao kod
    /// PlayerHonour.HonourId - nema nezavisan lookup endpoint za detalje samog milestone-a.
    /// </summary>
    public class PlayerMilestone : SyncedEntity.SyncedEntity
    {
        public long PlayerId { get; set; }
        public long TeamId { get; set; }
        public string? MilestoneId { get; set; }
        public string MilestoneName { get; set; }
        public string? MilestoneLogoUrl { get; set; }
        public DateOnly? DateAchieved { get; set; }

        public Player Player { get; set; }
        public Team Team { get; set; }
    }
}
