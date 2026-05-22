using Domain.Entities.ContentsMudule;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Presistence.Data.Configurations
{
    internal class TextContentConfigurations : IEntityTypeConfiguration<TextContent>
    {
        public void Configure(EntityTypeBuilder<TextContent> builder)
        {
            builder.Property(t => t.ViolentResult)
               .IsRequired()
               .HasDefaultValue(false);

            builder.Property(t => t.textContext)
                   .IsRequired()
                   .HasColumnType("nvarchar(max)");

            builder.Property(t => t.ViolentWords)
                   .HasConversion(
                       v => string.Join(',', v), // Convert List to string for saving
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() // Convert string to List for reading
                   );


        }
    }
}
