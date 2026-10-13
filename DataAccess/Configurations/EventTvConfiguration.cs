using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class EventTvConfiguration : SyncedEntityConfiguration<EventTv>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<EventTv> builder)
        {
            builder.Property(x => x.BroadcastTime).IsRequired();

            /* Match 1:N EventTv - ako se mec obrise, spisak prenosa nema smisla da ostane */
            builder.HasOne(x => x.Event)
                   .WithMany(x => x.TvBroadcasts)
                   .HasForeignKey(x => x.EventId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            /* Channel 1:N EventTv - Restrict, brisanje kanala ne sme da obori istoriju prenosa */
            builder.HasOne(x => x.Channel)
                   .WithMany(x => x.Broadcasts)
                   .HasForeignKey(x => x.ChannelId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            /* Sprecava duplirani prenos istog meca na istom kanalu */
            builder.HasIndex(x => new { x.EventId, x.ChannelId }).IsUnique();
        }
    }
}
