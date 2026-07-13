using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Employees;

namespace HospitalVacationManagement.Infrastructure.Persistence;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly InMemoryDatabase _database;

    public EmployeeRepository(InMemoryDatabase database)
    {
        _database = database;
    }

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = _database.Employees.FirstOrDefault(candidate => candidate.Id == id);

        return Task.FromResult(employee);
    }

    public Task<IReadOnlyCollection<Employee>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        var employees = _database.Employees
            .Where(candidate => candidate.DepartmentId == departmentId)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Employee>>(employees);
    }
}