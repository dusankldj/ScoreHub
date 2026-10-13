using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class EventTimelineConfiguration : SyncedEntityConfiguration<EventTimeline>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<EventTimeline> builder)
        {
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Detail).IsRequired(false);
            builder.Property(x => x.APIId).IsRequired(false);
            builder.Property(x => x.Comment).IsRequired(false);
            builder.Property(x => x.Period).IsRequired(false);

            /* Match 1:N EventTimeline - ako se mec obrise, njegov tok nema smisla da ostane */
            builder.HasOne(x => x.Event)
                   .WithMany(x => x.Timeline)
                   .HasForeignKey(x => x.EventId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            /* Restrict na Team/Player - brisanje tima/igraca ne sme tiho da obrise istoriju meca
               (i izbegava "multiple cascade paths" jer imamo dva FK-a ka Players) */
            builder.HasOne(x => x.Team)
                   .WithMany()
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(x => x.Player)
                   .WithMany()
                   .HasForeignKey(x => x.PlayerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(x => x.Assistant)
                   .WithMany()
                   .HasForeignKey(x => x.AssistantId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}
