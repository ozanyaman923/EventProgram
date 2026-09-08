using EventProgram.Application.Contracts.Events;

namespace EventProgram.Application.Abstractions.Events;

public interface IEventReadService
{
    Task<IReadOnlyList<EventSummary>> GetPublicEventsAsync(CancellationToken cancellationToken);
    Task<EventSummary?> GetByShareCodeAsync(string shareCode, CancellationToken cancellationToken);
}
