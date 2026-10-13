using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class PlayerHonourConfiguration : SyncedEntityConfiguration<PlayerHonour>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<PlayerHonour> builder)
        {
            builder.Property(x => x.HonourName).IsRequired();

            builder.HasOne(x => x.Player)
                   .WithMany(x => x.Honours)
                   .HasForeignKey(x => x.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.HasOne(x => x.Team)
                   .WithMany()
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(x => x.League)
                   .WithMany()
                   .HasForeignKey(x => x.LeagueId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}
