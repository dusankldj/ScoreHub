using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.BaseEntity;

namespace DataAccess.Configurations.BaseConfiguration
{
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x=>x.Id);

            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.UpdatedAt).IsRequired(false);

            ConfigureEntity(builder);
        }

        protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
    }


    public abstract class SyncedEntityConfiguration<TEntity> : BaseConfiguration<TEntity>
        where TEntity : Domain.Entities.SyncedEntity.SyncedEntity
    {
        protected sealed override void ConfigureEntity(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasIndex(x => x.ExternalId).IsUnique();

            ConfigureSyncedEntity(builder);
        }

        protected abstract void ConfigureSyncedEntity(EntityTypeBuilder<TEntity> builder);
    }
}
