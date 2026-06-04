using Domain.Entities.ContentsMudule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class TextContentConfiguration : IEntityTypeConfiguration<TextContent>
    {
        public void Configure(EntityTypeBuilder<TextContent> builder)
        {
            // Explicitly links this mapping class back to the base structure
            builder.HasBaseType<Content>();

            // Safely configure specialized entity properties
            builder.Property(t => t.ViolentWords).IsRequired(false);
            builder.Property(t => t.textContext).IsRequired(false);
        }
    }
}