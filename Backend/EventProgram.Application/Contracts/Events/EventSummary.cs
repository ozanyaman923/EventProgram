using EventProgram.Domain.Enums.Events;

namespace EventProgram.Application.Contracts.Events;

    public sealed record EventSummary(
        Guid Id,
        string Title,
        string Description,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        int? Capacity,
        EventVisibility Visibility,
        string ShareCode
    );
