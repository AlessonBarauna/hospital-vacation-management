using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Vacations;

public sealed record VacationRequestSummaryResponse(
    Guid Id,
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate,
    VacationRequestStatus Status);