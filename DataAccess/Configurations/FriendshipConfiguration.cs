using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class FriendshipConfiguration : BaseConfiguration.BaseConfiguration<Friendship>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Friendship> builder)
        {
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

            // Dva FK-a ka Users - NoAction (SQL Server ne dozvoljava dva cascade puta ka istoj tabeli); brisanje
            // korisnika zato mora prvo da ukloni njegova prijateljstva.
            builder.HasOne(x => x.Requester)
                   .WithMany()
                   .HasForeignKey(x => x.RequesterId)
                   .OnDelete(DeleteBehavior.NoAction)
                   .IsRequired();

            builder.HasOne(x => x.Addressee)
                   .WithMany()
                   .HasForeignKey(x => x.AddresseeId)
                   .OnDelete(DeleteBehavior.NoAction)
                   .IsRequired();

            // Isti par u istom smeru samo jednom. (Sprecavanje "obrnutog" duplikata je u kodu - vidi FriendsController.)
            builder.HasIndex(x => new { x.RequesterId, x.AddresseeId }).IsUnique();

            // "Prijatelji korisnika, poslednje dodati prvi" sa obe strane veze.
            builder.HasIndex(x => new { x.AddresseeId, x.Status, x.AcceptedAt });
            builder.HasIndex(x => new { x.RequesterId, x.Status, x.AcceptedAt });
        }
    }
}
