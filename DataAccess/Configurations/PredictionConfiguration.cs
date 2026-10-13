using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class PredictionConfiguration : BaseConfiguration.BaseConfiguration<Prediction>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Prediction> builder)
        {
            /* Prediction N:1 Match  */
            builder.HasOne(x => x.Match)
                   .WithMany(x => x.Predictions)
                   .HasForeignKey(x => x.MatchId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            /* Sprečava da isti korisnik unese dve prognoze za isti meč */
            builder.HasIndex(x => new { x.UserId, x.MatchId }).IsUnique();
        }
    }
}
