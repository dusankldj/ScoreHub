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

        // Soft delete: obrisan nalog ostaje u bazi (prognoze, istorija), ali se ne moze ulogovati i ne pojavljuje se u pretrazi,
        // profilima ni listama prijatelja. Username i e-mail ostaju zauzeti (unique indeksi).
        public bool IsDeleted { get; set; }

        // Trenutak soft delete-a: od njega se racuna rok u kome nalog moze da se vrati prijavom (Account:RestoreGraceDays),
        // a posle roka nalog ceka trajno brisanje. Null dok nalog nije obrisan.
        public DateTime? DeletedAt { get; set; }


        public ICollection<FavoriteTeam>? FavoriteTeams { get; set; }
        public ICollection<Prediction>? Predictions { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Jwt> Tokens { get; set; }
        public ICollection<DeviceToken>? DeviceTokens { get; set; }
        public ICollection<UserSearch>? UserSearches { get; set; }
    }
}
