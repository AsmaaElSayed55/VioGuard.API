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

            builder.HasOne(h => h.SystemRoot)
                   .WithMany(s => s.Histories)
                   .HasForeignKey(h => h.SystemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}