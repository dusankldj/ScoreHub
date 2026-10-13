using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class EventLineupConfiguration : SyncedEntityConfiguration<EventLineup>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<EventLineup> builder)
        {
            builder.Property(x => x.Position).IsRequired();
            builder.Property(x => x.PositionShort).IsRequired();

            /* Match 1:N EventLineup - ako se mec obrise, postava nema smisla da ostane */
            builder.HasOne(x => x.Event)
                   .WithMany(x => x.Lineups)
                   .HasForeignKey(x => x.EventId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            /* Player 1:N EventLineup - Restrict, ne zelimo da brisanje igraca obori istoriju postava */
            builder.HasOne(x => x.Player)
                   .WithMany()
                   .HasForeignKey(x => x.PlayerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            /* Team 1:N EventLineup - isto, Restrict */
            builder.HasOne(x => x.Team)
                   .WithMany()
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            /* Sprecava duplu postavu istog igraca u istom mecu */
            builder.HasIndex(x => new { x.EventId, x.PlayerId }).IsUnique();
        }
    }
}
