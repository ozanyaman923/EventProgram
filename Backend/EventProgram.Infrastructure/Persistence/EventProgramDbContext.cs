using EventProgram.Domain.Entities.Events;
using EventProgram.Domain.Entities.Users;
using EventProgram.Domain.Enums.Events;
using EventProgram.Domain.Enums.Users;
using Microsoft.EntityFrameworkCore;

namespace EventProgram.Infrastructure.Persistence;

public sealed class EventProgramDbContext(DbContextOptions<EventProgramDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.GoogleSubject).IsUnique();
            entity.Property(user => user.GoogleSubject).HasMaxLength(255).IsRequired();
            entity.Property(user => user.DisplayName).HasMaxLength(150).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(320).IsRequired();
            entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("events");
            entity.HasKey(eventItem => eventItem.Id);
            entity.HasIndex(eventItem => eventItem.ShareCode).IsUnique();
            entity.Property(eventItem => eventItem.Title).HasMaxLength(150).IsRequired();
            entity.Property(eventItem => eventItem.Description).HasMaxLength(5_000).IsRequired();
            entity.Property(eventItem => eventItem.ShareCode).HasMaxLength(64).IsRequired();
            entity.Property(eventItem => eventItem.AccessPasswordHash).HasMaxLength(255);
            entity.Property(eventItem => eventItem.Visibility).HasConversion<string>().HasMaxLength(20);
            entity.Property(eventItem => eventItem.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(eventItem => eventItem.Owner).WithMany().HasForeignKey(eventItem => eventItem.OwnerId).OnDelete(DeleteBehavior.Restrict);
        });

        SeedDemoEvents(modelBuilder);
    }

    private static void SeedDemoEvents(ModelBuilder modelBuilder)
    {
        var demoOwnerId = Guid.Parse("f7ac607a-36ed-4bb6-b952-b8fb76a1f764");
        var publicEventId = Guid.Parse("9b412d60-9f32-4c1e-b2b4-938c351d67fe");
        var privateEventId = Guid.Parse("76a7c9a3-5760-4c41-b272-6f08f2c9f203");
        var createdAtUtc = new DateTime(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<AppUser>().HasData(new { Id = demoOwnerId, GoogleSubject = "eventprogram-demo-owner", DisplayName = "EventProgram Demo", Email = "demo@eventprogram.local", Role = UserRole.User, IsBlocked = false, CreatedAtUtc = createdAtUtc });
        modelBuilder.Entity<Event>().HasData(
            new { Id = publicEventId, OwnerId = demoOwnerId, Title = "Yazılım Topluluğu Buluşması", Description = "Yeni başlayan geliştiriciler için proje fikri, portföy ve kariyer üzerine açık demo etkinliği.", StartsAtUtc = new DateTime(2030, 6, 15, 17, 0, 0, DateTimeKind.Utc), EndsAtUtc = new DateTime(2030, 6, 15, 19, 0, 0, DateTimeKind.Utc), Capacity = 80, Visibility = EventVisibility.Public, Status = EventStatus.Published, ShareCode = "8b52222e9e064b1badc429ed86f8f41a", AccessPasswordHash = (string?)null, CreatedAtUtc = createdAtUtc, UpdatedAtUtc = createdAtUtc },
            new { Id = privateEventId, OwnerId = demoOwnerId, Title = "EventProgram Kapalı Test Oturumu", Description = "Yalnızca davet bağlantısını alan kişiler için private etkinlik örneği.", StartsAtUtc = new DateTime(2030, 7, 10, 18, 0, 0, DateTimeKind.Utc), EndsAtUtc = new DateTime(2030, 7, 10, 19, 30, 0, 0, DateTimeKind.Utc), Capacity = 20, Visibility = EventVisibility.Private, Status = EventStatus.Published, ShareCode = "c8395aa8141c438d9c740627e8c493c3", AccessPasswordHash = (string?)null, CreatedAtUtc = createdAtUtc, UpdatedAtUtc = createdAtUtc });
    }
}
