using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class LeagueConfiguration : BaseConfiguration.SyncedEntityConfiguration<League>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<League> builder)
        {
            builder.Property(x=>x.Name).IsRequired();
            builder.Property(x => x.LogoUrl).IsRequired();
            builder.Property(x => x.CurrentSeason).IsRequired();

            /* League 1:N Team */
            builder.HasMany(x => x.Teams)
                   .WithOne(x => x.League)
                   .HasForeignKey(x => x.LeagueId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}
