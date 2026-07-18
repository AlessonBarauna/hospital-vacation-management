using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Users;

public sealed class UpdateUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponse?> HandleAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var userWithSameEmail = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (userWithSameEmail is not null && userWithSameEmail.Id != id)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        user.UpdateProfile(
            request.FullName,
            request.Email,
            request.Role);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            user.IsActive);
    }
}