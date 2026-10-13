using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Team : SyncedEntity.SyncedEntity
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string LogoUrl { get; set; }
        public long? StadiumId { get; set; }
        public string Country { get; set; }
        public long LeagueId { get; set; }


        public League League { get; set; }
        public ICollection<Match> HomeMatches { get; set; }
        public ICollection<Match> AwayMatches { get; set; }
        public ICollection<FavoriteTeam> FavoritedBy { get; set; }
        public ICollection<Player> Players { get; set; }
        public Venue Stadium { get; set; }
        public ICollection<TeamEquipment> Equipment { get; set; }
    }
}
