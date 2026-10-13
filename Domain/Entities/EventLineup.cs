using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class EventLineup : SyncedEntity.SyncedEntity
    {
        public long EventId { get; set; }
        public long PlayerId { get; set; }
        public long TeamId { get; set; }
        public long? ApiId { get; set; }
        public string Position { get; set; }
        public string PositionShort { get; set; }
        public string? Formation { get; set; }
        public bool HomeGame { get; set; } = false;
        public bool IsSubstitute { get; set; } = false;
        public int? SquadNumber { get; set; }
        public string? Season { get; set; }

        public Match Event { get; set; }
        public Player Player { get; set; }
        public Team Team { get; set; }
    }
}
