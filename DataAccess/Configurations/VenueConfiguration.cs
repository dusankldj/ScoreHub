using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class VenueConfiguration : BaseConfiguration.SyncedEntityConfiguration<Venue>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<Venue> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Location).IsRequired();
            builder.Property(x => x.Country).IsRequired();
            builder.Property(x => x.ImageUrl).IsRequired();

            /* Venue N:1 Sport - WithMany(x => x.Venues) eksplicitno spaja sa Sport.Venues
               kolekcijom, inace EF Core pravi DRUGU, dodatnu (shadow) vezu jer ne zna
               da su to iste dve strane iste relacije. */
            builder.HasOne(x => x.Sport)
                   .WithMany(x => x.Venues)
                   .HasForeignKey(x => x.SportId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}
