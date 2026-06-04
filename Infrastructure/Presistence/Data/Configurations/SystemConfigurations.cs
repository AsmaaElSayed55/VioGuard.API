using Domain.Entities.SystemModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    internal class SystemRootConfiguration : IEntityTypeConfiguration<SystemRoot>
    {
        public void Configure(EntityTypeBuilder<SystemRoot> builder)
        {
            builder.ToTable("SystemRoots");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.SystemName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}