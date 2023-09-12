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
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<UserLanguage>().ToTable("UserLanguage");
            modelBuilder.Entity<Language>().ToTable("Language");
            modelBuilder.Entity<Note>().ToTable("Note");
        }
    }
}
