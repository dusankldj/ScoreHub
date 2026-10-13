namespace Domain.Entities
{
    /// <summary>
    /// Osvojeni trofeji/priznanja igrača (lookup/player_honours/{id}). HonourId (idHonour) je
    /// SAMO informativna sekundarna oznaka iz API-ja - nemamo poseban "Honour" lookup/tabelu jer
    /// SportsDB ne nudi endpoint da se povuku detalji o njemu nezavisno (svi opisni podaci već
    /// stižu denormalizovano u ovom istom odgovoru - ime, logo, trofej).
    /// </summary>
    public class PlayerHonour : SyncedEntity.SyncedEntity
    {
        public long PlayerId { get; set; }
        public long TeamId { get; set; }
        public long? LeagueId { get; set; }
        public string? HonourId { get; set; }
        public string HonourName { get; set; }
        public string? HonourLogoUrl { get; set; }
        public string? HonourTrophyUrl { get; set; }
        public string? Season { get; set; }

        public Player Player { get; set; }
        public Team Team { get; set; }
        public League? League { get; set; }
    }
}
