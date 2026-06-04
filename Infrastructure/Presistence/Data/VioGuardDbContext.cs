using Domain.Entities.ContentsMudule;
using Microsoft.EntityFrameworkCore;

namespace Presistence.Data
{
    public class VioGuardDbContext : DbContext
    {
        public VioGuardDbContext(DbContextOptions<VioGuardDbContext> options) : base(options)
        {
        }

        public DbSet<Content> Contents { get; set; }

        // 🚨 ADD THESE TWO LINES SO EF CORE TRACKS THE INHERITED TYPES
        public DbSet<TextContent> TextContents { get; set; }
        public DbSet<VideoContent> VideoContents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Scans and applies your ContentConfiguration automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VioGuardDbContext).Assembly);
        }
    }
}