using EventProgram.Application.Abstractions.Events;
using EventProgram.Application.Contracts.Events;
using EventProgram.Domain.Entities.Events;
using EventProgram.Infrastructure.Persistence;

namespace EventProgram.Infrastructure.Services.Events;

public sealed class EventWriteManager(EventProgramDbContext dbContext) : IEventWriteService
{
    public async Task<EventSummary> CreateAsync(
        Guid ownerId,
        CreateEventRequest request,
        CancellationToken cancellationToken)
    {
        var eventItem = new Event(
            ownerId,
            request.Title,
            request.Description,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.Capacity,
            request.Visibility);

        eventItem.Publish();

        dbContext.Events.Add(eventItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new EventSummary(
            eventItem.Id,
            eventItem.Title,
            eventItem.Description,
            eventItem.StartsAtUtc,
            eventItem.EndsAtUtc,
            eventItem.Capacity,
            eventItem.Visibility,
            eventItem.ShareCode);
    }
}
