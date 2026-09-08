using EventProgram.Domain.Enums.Users;

namespace EventProgram.Domain.Entities.Users;

public sealed class AppUser
{
    private AppUser()
    {
    }

    public AppUser(string googleSubject, string displayName, string email)
    {
        Id = Guid.NewGuid();
        GoogleSubject = googleSubject;
        DisplayName = displayName;
        Email = email;
        Role = UserRole.User;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string GoogleSubject { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsBlocked { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
