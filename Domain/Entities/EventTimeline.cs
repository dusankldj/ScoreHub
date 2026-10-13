using System;

namespace Domain.Entities
{
    public class EventTimeline : SyncedEntity.SyncedEntity
    {
        public long EventId { get; set; }
        public string Type { get; set; }
        public string? Detail { get; set; }
        public bool Home { get; set; }
        public string? APIId { get; set; }
        public long? PlayerId { get; set; }
        public long? AssistantId { get; set; }
        public long TeamId { get; set; }
        public string? Comment { get; set; }
        public int? Minute { get; set; }
        public string? Period { get; set; }

        public Match Event { get; set; }
        public Player? Player { get; set; }
        public Player? Assistant { get; set; }
        public Team Team { get; set; }
    }
}
