using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.SyncedEntity
{
    public abstract class SyncedEntity : BaseEntity.BaseEntity
    {
        public string ExternalId { get; set; }
        public DateTime? LastSyncedAt { get; set; }
    }
}
