using Domain.Entities.ContentsMudule;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Data.Configurations
{
    internal class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.HasKey(c => c.URL);
            builder.Property(c => c.DetectionDate).HasDefaultValueSql("GETUTCDATE()");

            // TPH Configuration
            builder.HasDiscriminator<string>("ContentType")
                   .HasValue<TextContent>("Text")
                   .HasValue<VideoContent>("Video");

            builder.ToTable("Contents");


            // Relationship to User
            builder.HasOne(c => c.User)
                   .WithMany(u => u.Contents)
                   .HasForeignKey(c => c.UserEmail);

        }
    }
}
