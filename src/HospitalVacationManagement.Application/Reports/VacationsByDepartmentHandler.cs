using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Reports;

public sealed class VacationsByDepartmentHandler
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IVacationRequestRepository _vacationRequestRepository;

    public VacationsByDepartmentHandler(
        IDepartmentRepository departmentRepository,
        IVacationRequestRepository vacationRequestRepository)
    {
        _departmentRepository = departmentRepository;
        _vacationRequestRepository = vacationRequestRepository;
    }

    public async Task<IReadOnlyCollection<VacationsByDepartmentResponse>> HandleAsync(
        VacationsByDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var monthStart = new DateOnly(request.Year, request.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var departments = await _departmentRepository.GetAllAsync(cancellationToken);

        var vacations = await _vacationRequestRepository.ListByPeriodAsync(
            monthStart,
            monthEnd,
            cancellationToken);

        return departments
            .Select(department =>
            {
                var departmentVacations = vacations
                    .Where(vacationRequest => vacationRequest.DepartmentId == department.Id)
                    .ToList();

                return new VacationsByDepartmentResponse(
                    department.Id,
                    department.Name,
                    departmentVacations.Count(vacationRequest => vacationRequest.Status == VacationRequestStatus.Approved),
                    departmentVacations.Count(vacationRequest => vacationRequest.Status == VacationRequestStatus.Pending),
                    departmentVacations.Count(vacationRequest => vacationRequest.Status == VacationRequestStatus.Rejected),
                    departmentVacations.Count(vacationRequest => vacationRequest.Status == VacationRequestStatus.Cancelled));
            })
            .ToList();
    }
}