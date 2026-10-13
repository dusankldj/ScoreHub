using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class PlayerConfiguration : BaseConfiguration.SyncedEntityConfiguration<Player>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<Player> builder)
        {
            builder.Property(x => x.ImageUrl).IsRequired();
            builder.Property(x => x.BirthDate).IsRequired();
            builder.Property(x => x.BirthLocation).IsRequired();
            builder.Property(x => x.Position).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Weight).IsRequired();
            builder.Property(x => x.Height).IsRequired();


            builder.HasOne(x => x.Team)
                   .WithMany()
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.SetNull)
                   .IsRequired(false);

            // Restrict (ne SetNull kao gore) - dva FK-a na ISTU tabelu (Teams) sa cascade-nom
            // putanjom ka istom cilju bi SQL Server odbio ("multiple cascade paths"), pa samo
            // jedan od njih sme da bude SetNull/Cascade.
            builder.HasOne(x => x.SecondaryTeam)
                   .WithMany()
                   .HasForeignKey(x => x.SecondaryTeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);
        }
    }
}
