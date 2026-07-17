using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Departments;

namespace HospitalVacationManagement.Tests.Fakes;

public sealed class FakeDepartmentRepository : IDepartmentRepository
{
    private readonly List<Department> _departments = [];

    public void Add(Department department)
    {
        _departments.Add(department);
    }

    public Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = _departments.FirstOrDefault(candidate => candidate.Id == id);

        return Task.FromResult(department);
    }

    public Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<Department>>(_departments);
    }

    public Task AddAsync(Department department, CancellationToken cancellationToken)
    {
        _departments.Add(department);

        return Task.CompletedTask;
    }
}