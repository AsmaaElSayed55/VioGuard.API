using Domain.Entities.SystemModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Data.Configurations
{
    internal class HistoryRecordConfiguration : IEntityTypeConfiguration<HistoryRecord>
    {
        public void Configure(EntityTypeBuilder<HistoryRecord> builder)
        {
            builder.ToTable("Histories");

            builder.HasKey(h => h.Id);
            builder.Property(h => h.ContentUrl).IsRequired();
            builder.Property(h => h.ContentType).HasMaxLength(30);
            builder.Property(h => h.ActionDate).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(h => h.AttachedUserEmail).IsRequired();

            builder.HasOne(h => h.User)
                   .WithMany()
                   .HasForeignKey(h => h.AttachedUserEmail)
                   .HasPrincipalKey(u => u.Id)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
