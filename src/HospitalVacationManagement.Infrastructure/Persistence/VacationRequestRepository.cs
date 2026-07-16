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

    public async Task<VacationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.VacationRequests
            .FirstOrDefaultAsync(vacationRequest => vacationRequest.Id == id, cancellationToken);
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

    public async Task<IReadOnlyCollection<VacationRequest>> GetAllAsync(
    VacationRequestStatus? status,
    Guid? employeeId,
    IReadOnlyCollection<Guid>? employeeIds,
    DateOnly? startDate,
    DateOnly? endDate,
    CancellationToken cancellationToken)
{
    var query = _dbContext.VacationRequests.AsQueryable();

    if (status.HasValue)
    {
        query = query.Where(vacationRequest => vacationRequest.Status == status.Value);
    }

    if (employeeId.HasValue)
    {
        query = query.Where(vacationRequest => vacationRequest.EmployeeId == employeeId.Value);
    }

    if (employeeIds is not null && employeeIds.Count > 0)
    {
        query = query.Where(vacationRequest => employeeIds.Contains(vacationRequest.EmployeeId));
    }

    if (startDate.HasValue)
    {
        query = query.Where(vacationRequest => vacationRequest.EndDate >= startDate.Value);
    }

    if (endDate.HasValue)
    {
        query = query.Where(vacationRequest => vacationRequest.StartDate <= endDate.Value);
    }

    return await query
        .OrderBy(vacationRequest => vacationRequest.StartDate)
        .ToListAsync(cancellationToken);
}
}