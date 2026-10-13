using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Venue : SyncedEntity.SyncedEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string ImageUrl { get; set; }
        public long SportId { get; set; }


        public Sport Sport { get; set; }
        public ICollection<Team> Teams { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
