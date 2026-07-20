using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class RejectVacationRequestHandler
{
    private readonly IVacationRequestRepository _vacationRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectVacationRequestHandler(
        IVacationRequestRepository vacationRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _vacationRequestRepository = vacationRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ChangeVacationRequestStatusResponse> HandleAsync(
        Guid id,
        Guid currentUserId,
        CancellationToken cancellationToken)
    {
        var vacationRequest = await _vacationRequestRepository.GetByIdAsync(id, cancellationToken);

        if (vacationRequest is null)
        {
            return new ChangeVacationRequestStatusResponse(false, ["Vacation request was not found."]);
        }

        try
        {
            vacationRequest.Reject(currentUserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ChangeVacationRequestStatusResponse(true, []);
        }
        catch (InvalidOperationException exception)
        {
            return new ChangeVacationRequestStatusResponse(false, [exception.Message]);
        }
    }
}