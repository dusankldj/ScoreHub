using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class UserConfiguration : BaseConfiguration.BaseConfiguration<User>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
        {
            builder.Property(x=>x.FirstName).IsRequired();
            builder.Property(x=>x.LastName).IsRequired();
            builder.HasIndex(x => x.Username).IsUnique();
            builder.HasIndex(indexExpression: x => x.Email).IsUnique();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.TotalPoints).IsRequired(false);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);

            /* User 1:N FavoriteTeam */
            builder.HasMany(x => x.FavoriteTeams)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            /* User 1:N Prediction */
            builder.HasMany(x => x.Predictions)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            /* User N:1 UserRole */
            builder.HasOne(x => x.Role)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
        }
    }
}
