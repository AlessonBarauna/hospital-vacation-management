using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Users;

public sealed class GetUserByIdHandler
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new UserResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.IsActive
        );
    }
}