using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class DeviceTokenConfiguration : BaseConfiguration.BaseConfiguration<DeviceToken>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<DeviceToken> builder)
        {
            builder.Property(x => x.Token).IsRequired();

            /* DeviceToken N:1 User */
            builder.HasOne(x => x.User)
                   .WithMany(x => x.DeviceTokens)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            // Jedan fizicki uredjaj/instalacija = jedan token - re-login drugog korisnika na
            // istom uredjaju azurira UserId na postojecem redu umesto da pravi duplikat.
            builder.HasIndex(x => x.Token).IsUnique();
        }
    }
}
