using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class ListVacationRequestsHandler
{
    private readonly IVacationRequestRepository _vacationRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public ListVacationRequestsHandler(
        IVacationRequestRepository vacationRequestRepository,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository)
    {
        _vacationRequestRepository = vacationRequestRepository;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<IReadOnlyCollection<VacationRequestSummaryResponse>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var vacationRequests = await _vacationRequestRepository.GetAllAsync(cancellationToken);

        var response = new List<VacationRequestSummaryResponse>();

        foreach (var vacationRequest in vacationRequests)
        {
            var employee = await _employeeRepository.GetByIdAsync(
                vacationRequest.EmployeeId,
                cancellationToken);

            if (employee is null)
            {
                continue;
            }

            var department = await _departmentRepository.GetByIdAsync(
                employee.DepartmentId,
                cancellationToken);

            if (department is null)
            {
                continue;
            }

            response.Add(new VacationRequestSummaryResponse(
                vacationRequest.Id,
                employee.Id,
                employee.FullName,
                department.Id,
                department.Name,
                employee.Role,
                employee.SeniorityLevel,
                vacationRequest.StartDate,
                vacationRequest.EndDate,
                vacationRequest.Status));
        }

        return response;
    }
}