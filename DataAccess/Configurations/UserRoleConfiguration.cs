using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace DataAccess.Configurations
{
    public class UserRoleConfiguration : BaseConfiguration.BaseConfiguration<UserRole>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<UserRole> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();

            /* Seed - statične role, ne menjaju se dinamički. HasData ide direktno u migraciju,
               izvršava se pri database update, ne treba nikakav C# kod pri startu aplikacije. */
            builder.HasData(
                new UserRole { Id = 1, Name = "Admin", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new UserRole { Id = 2, Name = "User", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
