using Domain.Entities.SystemModule;
using Domain.Entities.SystemModule.ModelsModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class SystemConfigurations : IEntityTypeConfiguration<SystemRoot>
    {
        public void Configure(EntityTypeBuilder<SystemRoot> builder)
        {
            builder.HasKey(s => s.Id);

            // 1. One System to Many Histories Configuration
            builder.HasMany(s => s.Histories)
                   .WithOne(h => h.System)
                   .HasForeignKey(h => h.SystemId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 2. One System to Many AI Models Relationship Configuration
            builder.HasMany(s => s.Models)
                   .WithOne(m => m.System)
                   .HasForeignKey(m => m.SystemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class AIModelConfigurations : IEntityTypeConfiguration<AIModel>
    {
        public void Configure(EntityTypeBuilder<AIModel> builder)
        {
            // 🛠️ FIXED: Names now exactly match your C# class names (with underscores)
            builder.HasDiscriminator<string>("ModelType")
                   .HasValue<Text_Detect_Model>("Text")
                   .HasValue<Video_Detect_Model>("Video");

            builder.ToTable("AIModels");
        }
    }
}