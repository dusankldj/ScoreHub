using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class EventStatisticsConfiguration : SyncedEntityConfiguration<EventStatistic>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<EventStatistic> builder)
        {
            builder.Property(x=>x.EventName).IsRequired();
            builder.Property(x => x.Stat).IsRequired();
            builder.Property(x => x.HomeScore).IsRequired();
            builder.Property(x => x.AwayScore).IsRequired();


            /* Event 1:N EventStatistics */
            builder.HasOne(x => x.Event)
                    .WithMany(x => x.Statistics)
                    .HasForeignKey(x=>x.EventId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

            /* Sprecava duplu vrednost iste statistike (npr. dva "Shots on Goal" reda) za isti mec */
            builder.HasIndex(x => new { x.EventId, x.Stat }).IsUnique();
        }
    }
}
