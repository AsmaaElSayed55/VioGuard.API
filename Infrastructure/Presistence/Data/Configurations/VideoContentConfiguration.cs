using Domain.Entities.ContentsMudule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class VideoContentConfiguration : IEntityTypeConfiguration<VideoContent>
    {
        public void Configure(EntityTypeBuilder<VideoContent> builder)
        {
            // Explicitly links this mapping class back to the base structure
            builder.HasBaseType<Content>();

            // Safely configure specialized entity properties
            builder.Property(v => v.ViolentPercent).HasColumnType("float");
        }
    }
}