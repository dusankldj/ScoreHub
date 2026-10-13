using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class UserRole : BaseEntity.BaseEntity
    {
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
