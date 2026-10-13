using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User : BaseEntity.BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? TotalPoints { get; set; }
        public long RoleId { get; set; }


        public ICollection<FavoriteTeam>? FavoriteTeams { get; set; }
        public ICollection<Prediction>? Predictions { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Jwt> Tokens { get; set; }
        public ICollection<DeviceToken>? DeviceTokens { get; set; }
        public ICollection<UserSearch>? UserSearches { get; set; }
    }
}
