using System.ComponentModel.DataAnnotations;
using EventProgram.Domain.Enums.Events;

namespace EventProgram.Application.Contracts.Events;

public sealed record CreateEventRequest(
    [property: Required, StringLength(150)] string Title,
    [property: Required, StringLength(5_000)] string Description,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    [property: Range(1, int.MaxValue)] int? Capacity,
    EventVisibility Visibility) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndsAtUtc <= StartsAtUtc)
        {
            yield return new ValidationResult(
                "The end time must be after the start time.",
                [nameof(EndsAtUtc)]);
        }

        if (!Enum.IsDefined(Visibility))
        {
            yield return new ValidationResult(
                "The selected event visibility is invalid.",
                [nameof(Visibility)]);
        }
    }
}
