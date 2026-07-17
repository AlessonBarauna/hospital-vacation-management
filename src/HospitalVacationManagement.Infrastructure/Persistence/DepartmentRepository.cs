using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalVacationManagement.Infrastructure.Persistence;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _dbContext;

    public DepartmentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .FirstOrDefaultAsync(department => department.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .OrderBy(department => department.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Department department, CancellationToken cancellationToken)
    {
        await _dbContext.Departments.AddAsync(department, cancellationToken);
    }
}