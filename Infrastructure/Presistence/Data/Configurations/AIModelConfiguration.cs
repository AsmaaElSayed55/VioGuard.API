using Domain.Entities.SystemModule;
using Domain.Entities.SystemModule.ModelsModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    internal class AIModelConfiguration : IEntityTypeConfiguration<AIModel>
    {
        public void Configure(EntityTypeBuilder<AIModel> builder)
        {
            builder.ToTable("AIModels");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.Property(a => a.ModelType).HasMaxLength(50);
            builder.Property(a => a.Framework).HasMaxLength(50);

            builder.HasOne(a => a.SystemRoot)
                   .WithMany(s => s.AIModels)
                   .HasForeignKey(a => a.SystemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}