using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Abstractions;

public interface IVacationRequestRepository
{
    Task<IReadOnlyCollection<VacationRequest>> GetApprovedByDepartmentIdAsync(
        Guid departmentId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);
}