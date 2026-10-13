using Domain.Entities;
using Domain.Entities.SyncedEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class MatchConfiguration : BaseConfiguration.SyncedEntityConfiguration<Match>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<Match> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.EventDate).IsRequired();
            builder.Property(x => x.Season).IsRequired();
            builder.Property(x => x.Round).IsRequired();
            builder.Property(x => x.Status).IsRequired();

            
            builder.HasIndex(x => new { x.LeagueId, x.EventDate });
            builder.HasIndex(x => x.EventDate);
            builder.HasIndex(x => x.Status);


            /* Match N:1 League */
            builder.HasOne(x => x.League)
                   .WithMany()
                   .HasForeignKey(x => x.LeagueId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            /* Match N:1 Team (HomeTeam) */
            builder.HasOne(x => x.HomeTeam)
                   .WithMany(x => x.HomeMatches)
                   .HasForeignKey(x => x.HomeTeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            /* Match N:1 Team (AwayTeam) */
            builder.HasOne(x => x.AwayTeam)
                   .WithMany(x => x.AwayMatches)
                   .HasForeignKey(x => x.AwayTeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            /* Match N:1 Venue (opciono - Venue sync jos ne postoji) */
            builder.HasOne(x => x.Venue)
                .WithMany(x => x.Matches)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
