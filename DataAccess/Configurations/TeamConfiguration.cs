using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class TeamConfiguration : BaseConfiguration.SyncedEntityConfiguration<Team>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<Team> builder)
        {
            builder.Property(x=>x.Name).IsRequired();
            builder.Property(x => x.ShortName).IsRequired(false);
            builder.Property(x => x.LogoUrl).IsRequired();
            builder.Property(x => x.Country).IsRequired();

            /* Team 1:N Player */
            builder.HasMany(x => x.Players).WithOne(x => x.Team).HasForeignKey(x=>x.TeamId);

            /* Team N:1 Stadium */
            builder.HasOne(x => x.Stadium)
                .WithMany(x => x.Teams)
                .HasForeignKey(x => x.StadiumId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
