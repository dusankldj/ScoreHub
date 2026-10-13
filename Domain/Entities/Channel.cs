using System.Collections.Generic;

namespace Domain.Entities
{
    public class Channel : SyncedEntity.SyncedEntity
    {
        public string Name { get; set; }
        public string? Country { get; set; }
        public string? LogoUrl { get; set; }

        public ICollection<EventTv>? Broadcasts { get; set; }
    }
}
