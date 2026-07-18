namespace HospitalVacationManagement.Domain.Users;

public sealed class User
{
    public User(
        Guid id,
        string fullName,
        string email,
        string passwordHash,
        UserRole role)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public Guid Id { get; }

    public string FullName { get; }

    public string Email { get; }

    public string PasswordHash { get; }

    public UserRole Role { get; }
}