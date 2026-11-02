using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class SportScoringRuleConfiguration : BaseConfiguration.BaseConfiguration<SportScoringRule>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<SportScoringRule> builder)
        {
            builder.Property(x => x.ExactPoints).IsRequired();
            builder.Property(x => x.ClosePoints).IsRequired();
            builder.Property(x => x.WinnerPoints).IsRequired();
            builder.Property(x => x.CloseMarginTolerance).IsRequired(false);
            builder.Property(x => x.MaxScore).IsRequired(false);

            /* SportScoringRule 1:1 Sport (najvise jedan red po sportu) */
            builder.HasOne(x => x.Sport)
                   .WithMany()
                   .HasForeignKey(x => x.SportId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.HasIndex(x => x.SportId).IsUnique();
        }
    }
}
