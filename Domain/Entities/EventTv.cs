using System;

namespace Domain.Entities
{
    public class EventTv : SyncedEntity.SyncedEntity
    {
        public long EventId { get; set; }
        public long ChannelId { get; set; }
        public DateTime BroadcastTime { get; set; }

        public Match Event { get; set; }
        public Channel Channel { get; set; }
    }
}
