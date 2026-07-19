using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Users;

namespace HospitalVacationManagement.Tests.Fakes;

public sealed class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(user => user.Id == id);

        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(user => user.Email == email);

        return Task.FromResult(user);
    }

    public Task<IReadOnlyCollection<User>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<User>>(_users);
    }

    public Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        _users.Add(user);

        return Task.CompletedTask;
    }

    public void Update(User user)
    {
    }
}