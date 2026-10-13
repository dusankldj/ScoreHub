using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class TeamEquipment : SyncedEntity.SyncedEntity
    {
        public long TeamId { get; set; }
        public string Season { get; set; }
        public string Equipment { get; set; }
        public string Type { get; set; }
        public string Username { get; set; }


        public Team Team { get; set; }
    }
}
