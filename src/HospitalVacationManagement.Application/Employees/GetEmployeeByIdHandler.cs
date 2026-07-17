using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Employees;

public sealed class GetEmployeeByIdHandler
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeResponse?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);

        if (employee is null)
        {
            return null;
        }

        return new EmployeeResponse(
            employee.Id,
            employee.FullName,
            employee.DepartmentId,
            employee.Role,
            employee.SeniorityLevel);
    }
}