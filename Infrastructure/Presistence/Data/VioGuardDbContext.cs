using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Domain.Entities.UserModule;
using Microsoft.EntityFrameworkCore;

namespace Presistence.Data
{
    public class VioGuardDbContext : DbContext
    {
        public VioGuardDbContext(DbContextOptions<VioGuardDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<TextContent> TextContents { get; set; }
        public DbSet<VideoContent> VideoContents { get; set; }
        public DbSet<HistoryRecord> Histories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VioGuardDbContext).Assembly);
        }
    }
}
