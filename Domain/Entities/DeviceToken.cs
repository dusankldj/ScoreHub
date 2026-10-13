using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class DeviceToken : BaseEntity.BaseEntity
    {
        public long UserId { get; set; }
        public string Token { get; set; }

        public User User { get; set; }
    }
}
