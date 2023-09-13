using Microsoft.EntityFrameworkCore;
using MyOwnSummary_API.Models;

namespace MyOwnSummary_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Language> Languages { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLanguage> UserLanguages { get; set; }
        public DbSet<Note> Notes { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasIndex(x => x.UserName).IsUnique();
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");
                entity.HasIndex(x => x.Name).IsUnique();
            });
            modelBuilder.Entity<UserLanguage>(entity =>
            {
                entity.ToTable("UserLanguage");
                entity.HasAlternateKey(x => new {x.UserId,x.LanguageId});
            });
            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Language");
                entity.HasIndex(x => x.Name).IsUnique();
            });
            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("Note");
            });
        }
    }
}
