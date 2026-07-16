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
        Task<IReadOnlyCollection<VacationRequest>> GetAllAsync(CancellationToken cancellationToken);
        Task<VacationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}