using EventProgram.Application.Contracts.Events;

namespace EventProgram.Application.Abstractions.Events;

public interface IEventWriteService
{
    Task<EventSummary> CreateAsync(
        Guid ownerId,
        CreateEventRequest request,
        CancellationToken cancellationToken);
}
