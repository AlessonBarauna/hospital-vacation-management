using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class ListVacationRequestsHandler
{
    private readonly IVacationRequestRepository _vacationRequestRepository;

    public ListVacationRequestsHandler(IVacationRequestRepository vacationRequestRepository)
    {
        _vacationRequestRepository = vacationRequestRepository;
    }

    public async Task<IReadOnlyCollection<VacationRequestSummaryResponse>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var vacationRequests = await _vacationRequestRepository.GetAllAsync(cancellationToken);

        return vacationRequests
            .Select(vacationRequest => new VacationRequestSummaryResponse(
                vacationRequest.Id,
                vacationRequest.EmployeeId,
                vacationRequest.StartDate,
                vacationRequest.EndDate,
                vacationRequest.Status))
            .ToList();
    }
}