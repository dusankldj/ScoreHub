using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class UserSearchConfiguration : BaseConfiguration.BaseConfiguration<UserSearch>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<UserSearch> builder)
        {
            // Kao tekst ("Player"/"Team"/"League"), ne broj - citljivo u bazi i nezavisno od redosleda enum-a.
            builder.Property(x => x.Kind).HasConversion<string>().HasMaxLength(20).IsRequired();

            /* UserSearch N:1 User - brisanje korisnika brise i njegovu istoriju pretrage */
            builder.HasOne(x => x.User)
                   .WithMany(x => x.UserSearches)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            // Isto pretrazivanje ponovo = azurira SearchedAt postojeceg reda, nikad duplikat.
            builder.HasIndex(x => new { x.UserId, x.Kind, x.EntityId }).IsUnique();

            // "Poslednjih N za korisnika" - sortiranje po SearchedAt opadajuce.
            builder.HasIndex(x => new { x.UserId, x.SearchedAt });
        }
    }
}
