using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Users;

public sealed class ListUsersHandler
{
    private readonly IUserRepository _userRepository;

    public ListUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<UserResponse>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.ListAsync(cancellationToken);

        return users
            .Select(user => new UserResponse(
                user.Id,
                user.FullName,
                user.Email,
                user.Role,
                user.IsActive))
            .ToList();
    }
}