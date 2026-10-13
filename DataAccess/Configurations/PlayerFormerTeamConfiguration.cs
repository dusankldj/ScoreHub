using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class PlayerFormerTeamConfiguration : SyncedEntityConfiguration<PlayerFormerTeam>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<PlayerFormerTeam> builder)
        {
            builder.HasOne(x => x.Player)
                   .WithMany(x => x.FormerTeams)
                   .HasForeignKey(x => x.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.HasOne(x => x.FormerTeam)
                   .WithMany()
                   .HasForeignKey(x => x.FormerTeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}
