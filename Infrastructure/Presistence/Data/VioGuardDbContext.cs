
using Domain.Entities.ContentsMudule;
using Domain.Entities.UserModule;

namespace Presistence.Data
{
    public class VioGuardDbContext : DbContext
    {
        public VioGuardDbContext(DbContextOptions<VioGuardDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Content> Contents { get; set; }

    }
}
