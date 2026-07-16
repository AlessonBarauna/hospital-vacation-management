using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Domain.Vacations;
using HospitalVacationManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalVacationManagement.Infrastructure.Persistence;

public sealed class VacationRequestRepository : IVacationRequestRepository
{
    private readonly AppDbContext _dbContext;

    public VacationRequestRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(VacationRequest vacationRequest, CancellationToken cancellationToken)
{
    await _dbContext.VacationRequests.AddAsync(vacationRequest, cancellationToken);
}

    public async Task<IReadOnlyCollection<VacationRequest>> GetApprovedByDepartmentIdAsync(
        Guid departmentId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var employeeIds = await _dbContext.Employees
            .Where(employee => employee.DepartmentId == departmentId)
            .Select(employee => employee.Id)
            .ToListAsync(cancellationToken);

        return await _dbContext.VacationRequests
            .Where(vacation => vacation.Status == VacationRequestStatus.Approved)
            .Where(vacation => employeeIds.Contains(vacation.EmployeeId))
            .Where(vacation => vacation.StartDate <= endDate && startDate <= vacation.EndDate)
            .ToListAsync(cancellationToken);

    }

    public async Task<IReadOnlyCollection<VacationRequest>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.VacationRequests
            .OrderBy(vacationRequest => vacationRequest.StartDate)
            .ToListAsync(cancellationToken);
    }
}