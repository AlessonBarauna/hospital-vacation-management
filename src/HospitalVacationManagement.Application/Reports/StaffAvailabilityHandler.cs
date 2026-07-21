using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Reports;

public sealed class StaffAvailabilityHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IVacationRequestRepository _vacationRequestRepository;

    public StaffAvailabilityHandler(
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository,
        IVacationRequestRepository vacationRequestRepository)
    {
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
        _vacationRequestRepository = vacationRequestRepository;
    }

    public async Task<StaffAvailabilityResponse> xdxxxHandleAsync(
        StaffAvailabilityRequest request,
        CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(
            request.DepartmentId,
            cancellationToken);

        if (department is null)
        {
            throw new InvalidOperationException("Department was not found.");
        }

        var employees = await _employeeRepository.GetByDepartmentIdAsync(
            request.DepartmentId,
            cancellationToken);

        var vacationsInPeriod = await _vacationRequestRepository.ListByPeriodAsync(
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var approvedVacations = vacationsInPeriod
            .Where(vacationRequest =>
                vacationRequest.DepartmentId == request.DepartmentId &&
                vacationRequest.Status == VacationRequestStatus.Approved)
            .ToList();

        var employeeIdsOnVacation = approvedVacations
            .Select(vacationRequest => vacationRequest.EmployeeId)
            .ToHashSet();

        var availableEmployees = employees
            .Where(employee => !employeeIdsOnVacation.Contains(employee.Id))
            .ToList();

        var availableSeniorEmployees = availableEmployees
            .Count(employee => employee.SeniorityLevel == SeniorityLevel.Senior);

        return new StaffAvailabilityResponse(
            request.DepartmentId,
            department.Name,
            employees.Count(),
            employeeIdsOnVacation.Count,
            availableEmployees.Count,
            availableSeniorEmployees);
    }
    private readonly IDepartmentRepository _departmentRepository;
}