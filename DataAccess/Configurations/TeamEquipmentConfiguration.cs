using DataAccess.Configurations.BaseConfiguration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class TeamEquipmentConfiguration : SyncedEntityConfiguration<TeamEquipment>
    {
        protected override void ConfigureSyncedEntity(EntityTypeBuilder<TeamEquipment> builder)
        {
            builder.Property(x => x.Season).IsRequired();
            builder.Property(x => x.Equipment).IsRequired();
            builder.Property(x => x.Type).IsRequired();
            builder.Property(x => x.Username).IsRequired();

            /* Team 1:N Equipment */
            builder.HasOne(x => x.Team)
                   .WithMany(x => x.Equipment)
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();
        }
    }
}
