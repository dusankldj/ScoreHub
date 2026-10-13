namespace Domain.Entities
{
    /// <summary>
    /// Karijerna statistika igrača po sezoni/ligi/timu (lookup/player_stats/{id}).
    /// TeamId/LeagueId su opcioni - neki zapisi (npr. reprezentacija) mogu imati nepotpun kontekst.
    /// </summary>
    public class PlayerStatistic : SyncedEntity.SyncedEntity
    {
        public long PlayerId { get; set; }
        public long? TeamId { get; set; }
        public long? LeagueId { get; set; }
        public string Statistic { get; set; } // npr. "Appearances", "Goals"
        public string Value { get; set; }     // string namerno - nije uvek cist broj u svim sportovima
        public string? Season { get; set; }

        public Player Player { get; set; }
        public Team? Team { get; set; }
        public League? League { get; set; }
    }
}
