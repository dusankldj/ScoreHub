using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Jwt : BaseEntity.BaseEntity
    {
        public string Token { get; set; }
        public string RefreshJwtToken { get; set; }
        public bool IsRevoked { get; set; }
        public long UserId { get; set; }

        public User User { get; set; }
    }
}
