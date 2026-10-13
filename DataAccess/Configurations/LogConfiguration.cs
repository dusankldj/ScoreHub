using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class LogConfiguration : BaseConfiguration.BaseConfiguration<Log>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Log> builder)
        {
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.StatusCode);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
