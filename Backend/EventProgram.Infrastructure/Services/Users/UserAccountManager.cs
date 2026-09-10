using EventProgram.Application.Abstractions.Users;
using EventProgram.Application.Contracts.Authentication;
using EventProgram.Domain.Entities.Users;
using EventProgram.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventProgram.Infrastructure.Services.Users;

public sealed class UserAccountManager(EventProgramDbContext dbContext) : IUserAccountService
{
    public async Task<AuthenticatedUser> FindOrCreateGoogleUserAsync(
        string googleSubject,
        string displayName,
        string email,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(item => item.GoogleSubject == googleSubject, cancellationToken);

        if (user is null)
        {
            user = new AppUser(googleSubject, displayName, email);
            dbContext.Users.Add(user);
        }
        else
        {
            user.UpdateGoogleProfile(displayName, email);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticatedUser(user.Id, user.DisplayName, user.Email, user.IsBlocked);
    }
}
