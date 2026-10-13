namespace Domain.Entities
{
    public class Standings : SyncedEntity.SyncedEntity
    {
        public int Rank { get; set; }
        public long TeamId { get; set; }
        public long LeagueId { get; set; }
        public string Season { get; set; }
        public string? Group { get; set; }
        public string? Form { get; set; }
        public string? Description { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int MatchesDraw { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }

        public Team Team { get; set; }
        public League League { get; set; }
    }
}
