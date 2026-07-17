using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Employees;

public sealed class ListEmployeesHandler
{
    private readonly IEmployeeRepository _employeeRepository;

    public ListEmployeesHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IReadOnlyCollection<EmployeeResponse>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);

        return employees
            .Select(employee => new EmployeeResponse(
                employee.Id,
                employee.FullName,
                employee.DepartmentId,
                employee.Role,
                employee.SeniorityLevel))
            .ToList();
    }
}