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

    public async Task<ChangeVacationRequestStatusResponse> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var vacationRequest = await _vacationRequestRepository.GetByIdAsync(id, cancellationToken);

        if (vacationRequest is null)
        {
            return new ChangeVacationRequestStatusResponse(false, ["Vacation request was not found."]);
        }

        try
        {
            vacationRequest.Cancel();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ChangeVacationRequestStatusResponse(true, []);
        }
        catch (InvalidOperationException exception)
        {
            return new ChangeVacationRequestStatusResponse(false, [exception.Message]);
        }
    }
}