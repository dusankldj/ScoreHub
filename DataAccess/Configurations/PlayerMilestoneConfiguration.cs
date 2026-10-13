using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class PlayerMilestoneConfiguration : SyncedEntityConfiguration<PlayerMilestone>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<PlayerMilestone> builder)
        {
            builder.Property(x => x.MilestoneName).IsRequired();

            builder.HasOne(x => x.Player)
                   .WithMany(x => x.Milestones)
                   .HasForeignKey(x => x.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.HasOne(x => x.Team)
                   .WithMany()
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}
