using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class FavoriteTeamConfiguration : BaseConfiguration.BaseConfiguration<FavoriteTeam>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<FavoriteTeam> builder)
        {
            /* FavoriteTeam N:1 Team */
            builder.HasOne(x => x.Team)
                   .WithMany(x => x.FavoritedBy)
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            /* Sprečava da isti korisnik doda isti tim dva puta u omiljene */
            builder.HasIndex(x => new { x.UserId, x.TeamId }).IsUnique();
        }
    }
}
