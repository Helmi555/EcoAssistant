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
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<Mesure> Mesures => Set<Mesure>();

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

            e.HasMany(g => g.Devices)
            .WithOne(d => d.Group)
            .HasForeignKey(d => d.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<IndustryCategory>(e =>
        {

            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
        });
        builder.Entity<Device>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.Status).IsRequired();
            e.Property(d => d.Metadata).HasColumnType("jsonb");
            e.Property(d => d.CreatedAt).IsRequired();
            e.Property(d => d.UpdatedAt).IsRequired();
            e.HasMany(d => d.Sensors).WithOne(s => s.Device).HasForeignKey(s => s.DeviceId).OnDelete(DeleteBehavior.Cascade);
            e.ToTable("Devices");
            e.Property(d => d.PictureUrl).HasColumnType("text");
            e.Property(d=>d.DeviceName).HasMaxLength(100);
            e.Property(d=>d.Model).HasMaxLength(100);
            e.Property(d=>d.Manufacturer).HasMaxLength(100);
            e.Property(d => d.Version).HasMaxLength(50);
             e.HasOne(d => d.Group)
         .WithMany(g => g.Devices)
         .HasForeignKey(d => d.GroupId)
         .OnDelete(DeleteBehavior.SetNull);

        e.ToTable("Devices");
        });

        builder.Entity<Sensor>(e =>
        {
            // clé composite : DeviceId + LocalId
            e.HasKey(s => new { s.DeviceId, s.LocalId });

            e.Property(s => s.Type).IsRequired();
            e.Property(s => s.MinValue).IsRequired();
            e.Property(s => s.MaxValue).IsRequired();
            e.Property(s => s.AlertEnabled).IsRequired();
            e.Property(s => s.Metadata).HasColumnType("jsonb");
            e.Property(s => s.CreatedAt).IsRequired();
            e.Property(s => s.UpdatedAt).IsRequired();
            e.HasOne(s => s.Device).WithMany(d => d.Sensors).HasForeignKey(s => s.DeviceId).OnDelete(DeleteBehavior.Cascade);
            e.ToTable("Sensors");
        });

        builder.Entity<Mesure>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Value).IsRequired();
            e.Property(m => m.CreatedAt).IsRequired();
            e.HasOne(m => m.Sensor).WithMany().HasForeignKey(m => new { m.SensorDeviceId, m.SensorLocalId }).OnDelete(DeleteBehavior.Cascade);
            e.ToTable("Mesures");
        });
    }

    // Remplit LocalId pour les nouveaux capteurs : LocalId = max(existing.LocalId for Device) + 1
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var added = ChangeTracker.Entries<Sensor>().Where(e => e.State == EntityState.Added).Select(e => e.Entity).ToList();

        foreach (var s in added)
        {
            if (s.LocalId != 0) continue; // autorise assign manuel si souhaité

            // récupère le max existant dans la base pour cet appareil
            var maxForDevice = await Sensors
                .Where(x => x.DeviceId == s.DeviceId)
                .MaxAsync(x => (int?)x.LocalId, cancellationToken) ?? 0;

            s.LocalId = maxForDevice + 1;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

}

