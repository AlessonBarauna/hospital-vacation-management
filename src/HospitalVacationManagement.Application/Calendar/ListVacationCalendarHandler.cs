using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Calendar;

public sealed class ListVacationCalendarHandler
{
    private readonly IVacationRequestRepository _vacationRequestRepository;

    public ListVacationCalendarHandler(IVacationRequestRepository vacationRequestRepository)
    {
        _vacationRequestRepository = vacationRequestRepository;
    }

    public async Task<IReadOnlyCollection<VacationCalendarItemResponse>> HandleAsync(
        ListVacationCalendarRequest request,
        CancellationToken cancellationToken = default)
    {
        var monthStart = new DateOnly(request.Year, request.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var vacations = await _vacationRequestRepository.ListApprovedByMonthAsync(
            request.DepartmentId,
            monthStart,
            monthEnd,
            cancellationToken);

        return vacations
            .Select(vacationRequest => new VacationCalendarItemResponse(
                vacationRequest.Id,
                vacationRequest.EmployeeId,
                vacationRequest.DepartmentId,
                vacationRequest.StartDate,
                vacationRequest.EndDate,
                vacationRequest.Status))
            .ToList();
    }
}