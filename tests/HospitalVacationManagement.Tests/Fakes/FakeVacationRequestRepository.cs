using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Tests.Fakes;

public sealed class FakeVacationRequestRepository : IVacationRequestRepository
{
    private readonly List<VacationRequest> _vacationRequests = [];

    public IReadOnlyCollection<VacationRequest> VacationRequests => _vacationRequests;

    public void Add(VacationRequest vacationRequest)
    {
        _vacationRequests.Add(vacationRequest);
    }

    public Task<IReadOnlyCollection<VacationRequest>> GetApprovedByDepartmentIdAsync(
        Guid departmentId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var vacationRequests = _vacationRequests
            .Where(vacationRequest => vacationRequest.Status == VacationRequestStatus.Approved)
            .Where(vacationRequest => vacationRequest.Overlaps(startDate, endDate))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<VacationRequest>>(vacationRequests);
    }

    public Task AddAsync(VacationRequest vacationRequest, CancellationToken cancellationToken)
    {
        _vacationRequests.Add(vacationRequest);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<VacationRequest>> GetAllAsync(
        VacationRequestStatus? status,
        Guid? employeeId,
        IReadOnlyCollection<Guid>? employeeIds,
        DateOnly? startDate,
        DateOnly? endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<VacationRequest>>(_vacationRequests);
    }

    public Task<int> CountAsync(
        VacationRequestStatus? status,
        Guid? employeeId,
        IReadOnlyCollection<Guid>? employeeIds,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_vacationRequests.Count);
    }

    public Task<VacationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var vacationRequest = _vacationRequests.FirstOrDefault(candidate => candidate.Id == id);

        return Task.FromResult(vacationRequest);
    }

    public Task<bool> HasOverlappingRequestForEmployeeAsync(
        Guid employeeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var hasOverlap = _vacationRequests
            .Where(vacationRequest => vacationRequest.EmployeeId == employeeId)
            .Where(vacationRequest =>
                vacationRequest.Status == VacationRequestStatus.Pending ||
                vacationRequest.Status == VacationRequestStatus.Approved)
            .Any(vacationRequest => vacationRequest.Overlaps(startDate, endDate));

        return Task.FromResult(hasOverlap);
    }

    public Task<IReadOnlyCollection<VacationRequest>> ListApprovedByMonthAsync(
    Guid? departmentId,
    DateOnly monthStart,
    DateOnly monthEnd,
    CancellationToken cancellationToken)
{
    var vacations = _vacationRequests
        .Where(vacationRequest => vacationRequest.Status == VacationRequestStatus.Approved)
        .Where(vacationRequest =>
            vacationRequest.StartDate <= monthEnd &&
            vacationRequest.EndDate >= monthStart);

    if (departmentId.HasValue)
    {
        vacations = vacations.Where(vacationRequest =>
            vacationRequest.DepartmentId == departmentId.Value);
    }

    return Task.FromResult<IReadOnlyCollection<VacationRequest>>(
        vacations.ToList());
}
}