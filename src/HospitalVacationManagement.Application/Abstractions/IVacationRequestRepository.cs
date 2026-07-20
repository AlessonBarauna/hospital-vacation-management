using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Abstractions;

public interface IVacationRequestRepository
{
    Task<IReadOnlyCollection<VacationRequest>> GetApprovedByDepartmentIdAsync(
        Guid departmentId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
    Task AddAsync(VacationRequest vacationRequest, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<VacationRequest>> GetAllAsync(
    VacationRequestStatus? status,
    Guid? employeeId,
    IReadOnlyCollection<Guid>? employeeIds,
    DateOnly? startDate,
    DateOnly? endDate,
    int page,
    int pageSize,
    CancellationToken cancellationToken);

    Task<int> CountAsync(
        VacationRequestStatus? status,
        Guid? employeeId,
        IReadOnlyCollection<Guid>? employeeIds,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken);
    Task<VacationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> HasOverlappingRequestForEmployeeAsync(
        Guid employeeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<VacationRequest>> ListApprovedByMonthAsync(
        Guid? departmentId,
        DateOnly monthStart,
        DateOnly monthEnd,
        CancellationToken cancellationToken);

        Task<IReadOnlyCollection<VacationRequest>> ListByPeriodAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
}