using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class PlayerStatisticConfiguration : SyncedEntityConfiguration<PlayerStatistic>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<PlayerStatistic> builder)
        {
            builder.Property(x => x.Statistic).IsRequired();
            builder.Property(x => x.Value).IsRequired();

            /* Player 1:N PlayerStatistic - ako se igrac obrise, karijerna statistika nema smisla da ostane */
            builder.HasOne(x => x.Player)
                   .WithMany(x => x.Statistics)
                   .HasForeignKey(x => x.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            /* Team/League opcioni - Restrict da brisanje tima/lige ne obori istoriju */
            builder.HasOne(x => x.Team)
                   .WithMany()
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(x => x.League)
                   .WithMany()
                   .HasForeignKey(x => x.LeagueId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}
