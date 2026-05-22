using Domain.Entities.SystemModule;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Data.Configurations
{
    public class SystemConfigurations : IEntityTypeConfiguration<SystemRoot>
    {
        public void Configure(EntityTypeBuilder<SystemRoot> builder)
        {
            builder.HasKey(s => s.Id);

            // One System to Many Histories Configuration
            builder.HasMany(s => s.Histories)
                   .WithOne(h => h.System)
                   .HasForeignKey(h => h.SystemId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One System to Many AI Models Configuration
            builder.HasMany(s => s.Models)
                   .WithOne(m => m.System)
                   .HasForeignKey(m => m.SystemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
