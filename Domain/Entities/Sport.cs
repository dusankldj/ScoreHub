using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Sport : SyncedEntity.SyncedEntity
    {
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string? Description { get; set; }


        public ICollection<League> Leagues { get; set; }
        public ICollection<Venue> Venues { get; set; }
    }
}
