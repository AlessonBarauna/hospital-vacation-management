using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalVacationManagement.Infrastructure.Persistence;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Employees
            .FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Employee>> GetByDepartmentIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Employees
            .Where(employee => employee.DepartmentId == departmentId)
            .OrderBy(employee => employee.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Employees
            .OrderBy(employee => employee.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
    }
}