using Microsoft.EntityFrameworkCore;

namespace backend.Entities 
{
    public class FileShareDbContext : DbContext
    {
        public FileShareDbContext(DbContextOptions<FileShareDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Files)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);

            // Add validation to other USer fields
            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Token)
                .IsRequired();

            modelBuilder.Entity<File>()
                .HasIndex(f => f.Path)
                .IsUnique();

            modelBuilder.Entity<File>()
                .Property(f => f.Name)
                .IsRequired();

            modelBuilder.Entity<File>()
                .Property(f => f.UserId)
                .IsRequired();

            modelBuilder.Entity<File>()
                .Property(f => f.Path)
                .IsRequired();


        }
    }
}
