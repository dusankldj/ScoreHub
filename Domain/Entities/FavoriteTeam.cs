using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class FavoriteTeam : BaseEntity.BaseEntity
    {
        public long UserId { get; set; }
        public long TeamId { get; set; }
        public bool Notifications { get; set; } = false;


        public User User { get; set; }
        public Team Team { get; set; }
        /* veza many-many? */
    }
}
