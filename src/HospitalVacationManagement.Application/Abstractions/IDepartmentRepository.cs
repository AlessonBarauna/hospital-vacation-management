using HospitalVacationManagement.Domain.Departments;

namespace HospitalVacationManagement.Application.Abstractions;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Department department, CancellationToken cancellationToken);
}