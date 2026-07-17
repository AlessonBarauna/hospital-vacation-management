using HospitalVacationManagement.Domain.Employees;

namespace HospitalVacationManagement.Application.Abstractions;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Employee>> GetByDepartmentIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Employee employee, CancellationToken cancellationToken);
}