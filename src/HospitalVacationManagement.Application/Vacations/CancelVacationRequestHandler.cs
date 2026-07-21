using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class CancelVacationRequestHandler
{
    private readonly IVacationRequestRepository _vacationRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelVacationRequestHandler(
        IVacationRequestRepository vacationRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _vacationRequestRepository = vacationRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangeVacationRequestStatusResponse?> HandleAsync(
        Guid vacationRequestId,
        Guid cancelledByUserId,
        CancellationToken cancellationToken)
    {
        var vacationRequest = await _vacationRequestRepository.GetByIdAsync(
            vacationRequestId,
            cancellationToken);

        if (vacationRequest is null)
        {
            return null;
        }

        vacationRequest.Cancel(cancelledByUserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeVacationRequestStatusResponse(
            true,
            []);
    }
}