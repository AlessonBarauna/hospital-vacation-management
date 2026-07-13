using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Infrastructure.Persistence;

public sealed class VacationRequestRepository : IVacationRequestRepository
{
    private readonly InMemoryDatabase _database;

    public VacationRequestRepository(InMemoryDatabase database)
    {
        _database = database;
    }

    public Task<IReadOnlyCollection<VacationRequest>> GetApprovedByDepartmentIdAsync(
        Guid departmentId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var vacationRequests = _database.VacationRequests
            .Where(vacation => vacation.Status == VacationRequestStatus.Approved)
            .Where(vacation => vacation.Overlaps(startDate, endDate))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<VacationRequest>>(vacationRequests);
    }
}