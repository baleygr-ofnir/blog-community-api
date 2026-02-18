using System.Runtime.CompilerServices;
using blog_community_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace blog_community_api.Data;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(128);

            entity.Property(u => u.PasswordHash)
                .IsRequired();
            
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            
            entity.Property(u => u.CreatedAt)
                .IsRequired();
            
            entity.Property(u => u.UpdatedAt)
                .IsRequired(false);
            
            entity.HasIndex(u => u.Username)
                .IsUnique();
            
            entity.HasIndex(u => u.Email)
                .IsUnique();
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(bp => bp.Id);
            
            entity.Property(bp => bp.Title)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(bp => bp.Content)
                .IsRequired()
                .HasMaxLength(512);
            
            entity.Property(bp => bp.CreatedAt)
                .IsRequired();
            
            entity.Property(bp => bp.UpdatedAt)
                .IsRequired(false);
            
            entity.HasOne(bp => bp.User)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(bp => bp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(bp => bp.Category)
                .WithMany(c => c.BlogPosts)
                .HasForeignKey(bp => bp.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(128);
            
            entity.HasIndex(c => c.Name)
                .IsUnique();
            
            entity.HasMany(c => c.BlogPosts)
                .WithOne(bp => bp.Category)
                .HasForeignKey(bp => bp.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(bc => bc.Id);

            entity.Property(bc => bc.Content)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(bc => bc.CreatedAt)
                .IsRequired();
            
            entity.Property(bc => bc.UpdatedAt)
                .IsRequired(false);
            
            entity.HasOne(bc => bc.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(bc => bc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(bc => bc.BlogPost)
                .WithMany(bp => bp.Comments)
                .HasForeignKey(bc => bc.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}