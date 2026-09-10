using EventProgram.Application.Contracts.Authentication;

namespace EventProgram.Application.Abstractions.Users;

public interface IUserAccountService
{
    Task<AuthenticatedUser> FindOrCreateGoogleUserAsync(
        string googleSubject,
        string displayName,
        string email,
        CancellationToken cancellationToken);
}
