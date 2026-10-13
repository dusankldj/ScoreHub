using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class SportConfiguration : BaseConfiguration.SyncedEntityConfiguration<Sport>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<Sport> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.IconUrl).IsRequired();
            builder.Property(x => x.Description).IsRequired(false);

            /* Sport 1:N League */
            builder.HasMany(x => x.Leagues).WithOne(x => x.Sport).HasForeignKey(x => x.SportId);

            /* Sport 1:N Venue */
            builder.HasMany(x => x.Venues)
                    .WithOne(x => x.Sport)
                    .HasForeignKey(x=>x.SportId);
        }
    }
}
