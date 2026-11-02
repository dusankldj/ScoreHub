using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class EmailVerificationCodeConfiguration : BaseConfiguration.BaseConfiguration<EmailVerificationCode>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<EmailVerificationCode> builder)
        {
            builder.Property(x => x.Purpose).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(x => x.TargetEmail).HasMaxLength(256).IsRequired();
            builder.Property(x => x.CodeHash).HasMaxLength(200).IsRequired();

            /* N:1 User - brisanje korisnika brise i njegove kodove */
            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            // "Poslednji kod korisnika za tu svrhu" (pauza za Resend, ogranicenje slanja, provera koda).
            builder.HasIndex(x => new { x.UserId, x.Purpose, x.CreatedAt });
        }
    }
}
