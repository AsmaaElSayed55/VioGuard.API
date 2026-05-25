using Domain.Entities.ContentsMudule; // Matches your solution spelling namespace
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            // 1. Base Table Configurations
            builder.ToTable("Contents");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("URL");

            builder.Property(c => c.DetectionDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            // 2. Table-Per-Hierarchy (TPH) Discriminator Definition
            builder.HasDiscriminator<string>("ContentType")
                   .HasValue<TextContent>("Text")
                   .HasValue<VideoContent>("Video");
        }
    }
}