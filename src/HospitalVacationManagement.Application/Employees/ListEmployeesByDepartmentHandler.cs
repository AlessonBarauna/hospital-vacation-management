using HospitalVacationManagement.Application.Abstractions;

namespace HospitalVacationManagement.Application.Employees;

public sealed class ListEmployeesByDepartmentHandler
{
    private readonly IEmployeeRepository _employeeRepository;

    public ListEmployeesByDepartmentHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IReadOnlyCollection<EmployeeResponse>> HandleAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetByDepartmentIdAsync(
            departmentId,
            cancellationToken);

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