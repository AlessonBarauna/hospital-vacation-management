using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Departments;

namespace HospitalVacationManagement.Infrastructure.Persistence;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly InMemoryDatabase _database;

    public DepartmentRepository(InMemoryDatabase database)
    {
        _database = database;
    }

    public Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = _database.Departments.FirstOrDefault(candidate => candidate.Id == id);

        return Task.FromResult(department);
    }
}