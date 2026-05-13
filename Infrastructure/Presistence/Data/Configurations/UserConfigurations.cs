using Domain.Entities.UserModule;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Data.Configurations
{
    internal class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Email);
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Password).IsRequired();

            // Relationship: One User to Many Contents
            builder.HasMany(u => u.Contents)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserEmail)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.ToTable("Users");

        }
    }
}
