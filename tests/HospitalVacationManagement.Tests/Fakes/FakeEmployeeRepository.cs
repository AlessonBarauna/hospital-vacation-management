using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Employees;

namespace HospitalVacationManagement.Tests.Fakes;

public sealed class FakeEmployeeRepository : IEmployeeRepository
{
    private readonly List<Employee> _employees = [];

    public void Add(Employee employee)
    {
        _employees.Add(employee);
    }

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = _employees.FirstOrDefault(candidate => candidate.Id == id);

        return Task.FromResult(employee);
    }

    public Task<IReadOnlyCollection<Employee>> GetByDepartmentIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var employees = _employees
            .Where(candidate => candidate.DepartmentId == departmentId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Employee>>(employees);
    }

    public Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<Employee>>(_employees);
    }

    public Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        _employees.Add(employee);

        return Task.CompletedTask;
    }
}