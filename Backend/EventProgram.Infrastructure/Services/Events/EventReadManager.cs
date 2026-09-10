using EventProgram.Application.Abstractions.Events;
using EventProgram.Application.Contracts.Events;
using EventProgram.Domain.Entities.Events;
using EventProgram.Domain.Enums.Events;
using EventProgram.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventProgram.Infrastructure.Services.Events;

public sealed class EventReadManager(EventProgramDbContext dbContext) : IEventReadService
{
    public async Task<IReadOnlyList<EventSummary>> GetPublicEventsAsync(CancellationToken cancellationToken) =>
        await dbContext.Events.AsNoTracking()
            .Where(eventItem => eventItem.Visibility == EventVisibility.Public && eventItem.Status == EventStatus.Published)
            .OrderBy(eventItem => eventItem.StartsAtUtc)
            .Select(ToSummary())
            .ToListAsync(cancellationToken);

    public async Task<EventSummary?> GetByShareCodeAsync(string shareCode, CancellationToken cancellationToken) =>
        await dbContext.Events.AsNoTracking()
            .Where(eventItem => eventItem.ShareCode == shareCode)
            .Select(ToSummary())
            .SingleOrDefaultAsync(cancellationToken);

    private static System.Linq.Expressions.Expression<Func<Event, EventSummary>> 
        ToSummary() => eventItem => new EventSummary(
        eventItem.Id, eventItem.Title, eventItem.Description, eventItem.StartsAtUtc, eventItem.EndsAtUtc,
        eventItem.Capacity, eventItem.Visibility, eventItem.ShareCode);
}
