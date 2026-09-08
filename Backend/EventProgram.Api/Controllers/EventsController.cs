using EventProgram.Application.Abstractions.Events;
using EventProgram.Application.Contracts.Events;
using Microsoft.AspNetCore.Mvc;

namespace EventProgram.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(IEventReadService eventReadService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventSummary>>> GetPublicEvents(
        CancellationToken cancellationToken)
    {
        var events = await eventReadService.GetPublicEventsAsync(cancellationToken);

        return Ok(events);
    }

    [HttpGet("{shareCode}")]
    public async Task<ActionResult<EventSummary>> GetByShareCode(
        string shareCode,
        CancellationToken cancellationToken)
    {
        var eventItem = await eventReadService.GetByShareCodeAsync(shareCode, cancellationToken);

        return eventItem is null ? NotFound() : Ok(eventItem);
    }
}
