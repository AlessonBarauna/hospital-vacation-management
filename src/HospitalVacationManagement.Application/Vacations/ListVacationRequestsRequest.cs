using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Vacations;

public sealed record ListVacationRequestsRequest(
    VacationRequestStatus? Status,
    Guid? EmployeeId,
    Guid? DepartmentId,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int Page,
    int PageSize);