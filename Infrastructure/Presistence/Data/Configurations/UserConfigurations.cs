using Domain.Entities.UserModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            // Map base 'Id' property directly to the 'Email' column
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("Email").HasMaxLength(256);

            builder.Property(u => u.UserInternalId).HasColumnName("Id").IsRequired();
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Password).IsRequired();
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}