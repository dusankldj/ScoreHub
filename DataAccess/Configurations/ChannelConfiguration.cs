using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ChannelConfiguration : SyncedEntityConfiguration<Channel>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<Channel> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Country).IsRequired(false);
            builder.Property(x => x.LogoUrl).IsRequired(false);
        }
    }
}
