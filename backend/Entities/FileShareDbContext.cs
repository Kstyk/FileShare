using Microsoft.EntityFrameworkCore;

namespace backend.Entities 
{
    public class FileShareDbContext : DbContext
    {
        public FileShareDbContext(DbContextOptions<FileShareDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
