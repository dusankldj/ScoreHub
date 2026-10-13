namespace Domain.Entities
{
    /// <summary>
    /// Istorija transfera igrača (lookup/player_teams/{id}).
    /// </summary>
    public class PlayerFormerTeam : SyncedEntity.SyncedEntity
    {
        public long PlayerId { get; set; }
        public long FormerTeamId { get; set; }
        public string? MoveType { get; set; }   // "Permanent"/"Loan"
        public string? BadgeUrl { get; set; }
        public string? Joined { get; set; }     // godina, kao tekst (API tako vraca)
        public string? Departed { get; set; }
        public int? Appearances { get; set; }
        public int? Goals { get; set; }

        public Player Player { get; set; }
        public Team FormerTeam { get; set; }
    }
}
