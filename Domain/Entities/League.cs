using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class League : SyncedEntity.SyncedEntity
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string LogoUrl { get; set; }
        public string CurrentSeason { get; set; }
        public long SportId { get; set; }


        public ICollection<Team> Teams { get; set; }
        public Sport Sport { get; set; }
    }
}
