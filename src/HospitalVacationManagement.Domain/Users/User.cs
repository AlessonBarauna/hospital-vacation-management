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
        IsActive = true;
    }

    public void UpdateProfile(
        string fullName,
        string email,
        UserRole role)
    {
        FullName = fullName;
        Email = email;
        Role = role;
    }

    public void ChangePassword(
        string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public Guid Id { get; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
}