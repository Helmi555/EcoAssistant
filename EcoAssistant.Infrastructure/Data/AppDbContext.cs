using System.Dynamic;
using EcoAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoAssistant.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<Group> Groups => Set<Group>();

    public DbSet<IndustryCategory> IndustryCategories => Set<IndustryCategory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
            e.Property(x => x.Address).HasMaxLength(500);

            e.HasMany(u => u.UserGroups)
            .WithOne(ug => ug.User)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        });

        builder.Entity<UserGroup>(e =>
        {

            e.HasKey(x => new { x.UserId, x.GroupId });

            e.Property(x => x.Role).IsRequired();
            e.Property(x => x.Status).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();

            e.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Group)
             .WithMany(g => g.UserGroups)
             .HasForeignKey(x => x.GroupId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Group>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();

            e.HasOne(g => g.IndustryCategory)
             .WithMany()
             .HasForeignKey(g => g.IndustryCategoryId)
             .OnDelete(DeleteBehavior.SetNull);

            e.HasMany(g => g.UserGroups)
             .WithOne(ug => ug.Group)
             .HasForeignKey(ug => ug.GroupId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<IndustryCategory>(e =>
        {

            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
        });

    }


}