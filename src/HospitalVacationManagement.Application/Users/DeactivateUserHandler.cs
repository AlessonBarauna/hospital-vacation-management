using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Users;

public sealed class DeactivateUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.Deactivate();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}