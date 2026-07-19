using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Users;

public sealed class ChangeOwnPasswordHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeOwnPasswordHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangeOwnPasswordResult> HandleAsync(
        Guid userId,
        ChangeOwnPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return ChangeOwnPasswordResult.UserNotFound;
        }

        var currentPasswordIsValid = _passwordHasher.Verify(
            request.CurrentPassword,
            user.PasswordHash);

        if (!currentPasswordIsValid)
        {
            return ChangeOwnPasswordResult.InvalidCurrentPassword;
        }

        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);

        user.ChangePassword(newPasswordHash);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ChangeOwnPasswordResult.Success;
    }
}