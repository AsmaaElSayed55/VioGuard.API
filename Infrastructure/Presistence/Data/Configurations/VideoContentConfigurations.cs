using Domain.Entities.ContentsMudule;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Data.Configurations
{
    internal class VideoContentConfigurations : IEntityTypeConfiguration<VideoContent>
    {
        public void Configure(EntityTypeBuilder<VideoContent> builder)
        {
            builder.Property(v => v.ViolentPercent)
               .IsRequired()
               .HasDefaultValue(0.0)
               .HasColumnType("decimal(5,2)");

        }
    }
}
