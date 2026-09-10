namespace EventProgram.Application.Contracts.Authentication;

public sealed record AuthenticatedUser(
    Guid Id,
    string DisplayName,
    string Email,
    bool IsBlocked);
