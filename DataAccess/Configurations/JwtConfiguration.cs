using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class JwtConfiguration : BaseConfiguration.BaseConfiguration<Jwt>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Jwt> builder)
        {
            builder.Property(x => x.Token).IsRequired();
            builder.Property(x => x.RefreshJwtToken).IsRequired();
            builder.Property(x => x.IsRevoked).IsRequired().HasDefaultValue(false);
            builder.HasIndex(x => x.RefreshJwtToken).IsUnique();

            /* Jwt N:1 User */
            builder.HasOne(x => x.User)
                   .WithMany(x => x.Tokens)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            
            /* KLJUČNO: garantuje na nivou baze da korisnik ima NAJVIŠE JEDAN aktivan
               (ne-opozvan) token u isto vreme. Filtrirani indeks - unique pravilo važi
               samo za redove gde je IsRevoked = 0, opozvani/istorijski redovi mogu
               slobodno da postoje u više primeraka.
               NAPOMENA: ovo znači da login na novom uređaju/pregledaču opoziva sve
               prethodne sesije tog korisnika (single-session model). Ako kasnije poželiš
               da korisnik može da bude ulogovan na više uređaja istovremeno, ovaj indeks
               treba ukloniti i "najviše jedan validan" prebaciti isključivo u aplikativnu
               logiku (npr. jedan validan token PO UREĐAJU, ne po korisniku). */
            builder.HasIndex(x => x.UserId)
                   .HasFilter("[IsRevoked] = 0")
                   .IsUnique();
        }
    }
}
