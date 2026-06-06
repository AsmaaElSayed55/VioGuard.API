using Domain.Entities.ContentsMudule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.ToTable("Contents");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("URL");

            builder.Property(c => c.URL)
                   .HasColumnName("URL1")
                   .IsRequired();

            builder.Property(c => c.UserEmail).IsRequired();
            builder.Property(c => c.DetectionDate).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(c => c.User)
                   .WithMany(u => u.Contents)
                   .HasForeignKey(c => c.UserEmail)
                   .HasPrincipalKey(u => u.Id)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasDiscriminator<string>("ContentType")
                   .HasValue<TextContent>("Text")
                   .HasValue<VideoContent>("Video");
        }
    }
}
