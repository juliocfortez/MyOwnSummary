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

        public DbSet<Role> Roles { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasIndex(x => x.UserName).IsUnique();
                entity.Property(x=>x.UserName).IsRequired();
                entity.Property(x => x.Password).IsRequired();
                entity.HasOne(x=>x.Role).WithMany(x=>x.Users).HasForeignKey(x=>x.RoleId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");
                entity.HasIndex(x => x.Name).IsUnique();
                entity.Property(x => x.Name).IsRequired();
            });
            modelBuilder.Entity<UserLanguage>(entity =>
            {
                entity.ToTable("UserLanguage");
                entity.HasAlternateKey(x => new {x.UserId,x.LanguageId});
                entity.HasOne(x=>x.Language).WithMany(x=>x.Users).HasForeignKey(x=>x.LanguageId).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(x=>x.User).WithMany(x=>x.Languages).HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.NoAction); 
            });
            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Language");
                entity.HasIndex(x => x.Name).IsUnique();
                entity.Property(x => x.Name).IsRequired();
            });
            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("Note");
                entity.Property(x => x.Description).IsRequired();
                entity.HasOne(x => x.User).WithMany(x => x.Notes).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.Category).WithMany(x=>x.Notes).HasForeignKey(x=>x.CategoryId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x=>x.Language).WithMany(x=>x.Notes).HasForeignKey(x=>x.LanguageId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");
                entity.HasIndex(x => x.Name).IsUnique();
                entity.Property(x => x.Name).IsRequired();
            });
        }
    }
}
