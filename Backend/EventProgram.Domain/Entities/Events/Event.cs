using System.Security.Cryptography;
using EventProgram.Domain.Entities.Users;
using EventProgram.Domain.Enums.Events;

namespace EventProgram.Domain.Entities.Events;

public sealed class Event
{
    private Event()
    {
    }

    public Event(Guid ownerId, string title, string description, DateTime startsAtUtc, DateTime endsAtUtc, int? capacity, EventVisibility visibility)
    {
        if (ownerId == Guid.Empty) throw new ArgumentException("An event must have an owner.", nameof(ownerId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("A title is required.", nameof(title));
        if (endsAtUtc <= startsAtUtc) throw new ArgumentException("The end time must be after the start time.", nameof(endsAtUtc));
        if (capacity is <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

        Id = Guid.NewGuid();
        OwnerId = ownerId;
        Title = title.Trim();
        Description = description.Trim();
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Capacity = capacity;
        Visibility = visibility;
        Status = EventStatus.Draft;
        ShareCode = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public AppUser Owner { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public int? Capacity { get; private set; }
    public EventVisibility Visibility { get; private set; }
    public EventStatus Status { get; private set; }
    public string ShareCode { get; private set; } = null!;
    public string? AccessPasswordHash { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Publish()
    {
        if (Status != EventStatus.Draft)
        {
            throw new InvalidOperationException("Only draft events can be published.");
        }

        Status = EventStatus.Published;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
