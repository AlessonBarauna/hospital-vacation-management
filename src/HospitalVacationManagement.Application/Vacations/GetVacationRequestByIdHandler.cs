using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class GetVacationRequestByIdHandler
{
    private readonly IVacationRequestRepository _vacationRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public GetVacationRequestByIdHandler(
        IVacationRequestRepository vacationRequestRepository,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository)
    {
        _vacationRequestRepository = vacationRequestRepository;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<VacationRequestSummaryResponse?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var vacationRequest = await _vacationRequestRepository.GetByIdAsync(id, cancellationToken);

        if (vacationRequest is null)
        {
            return null;
        }

        var employee = await _employeeRepository.GetByIdAsync(
            vacationRequest.EmployeeId,
            cancellationToken);

        if (employee is null)
        {
            return null;
        }

        var department = await _departmentRepository.GetByIdAsync(
            employee.DepartmentId,
            cancellationToken);

        if (department is null)
        {
            return null;
        }

        return new VacationRequestSummaryResponse(
            vacationRequest.Id,
            employee.Id,
            employee.FullName,
            department.Id,
            department.Name,
            employee.Role,
            employee.SeniorityLevel,
            vacationRequest.StartDate,
            vacationRequest.EndDate,
            vacationRequest.CreatedAt,
            vacationRequest.UpdatedAt,
            vacationRequest.Status);
    }
}