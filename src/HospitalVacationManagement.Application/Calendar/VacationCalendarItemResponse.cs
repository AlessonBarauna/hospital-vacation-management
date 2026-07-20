using HospitalVacationManagement.Domain.Vacations;

namespace HospitalVacationManagement.Application.Calendar;

public sealed record VacationCalendarItemResponse(
    Guid VacationRequestId,
    Guid EmployeeId,
    Guid DepartmentId,
    DateOnly StartDate,
    DateOnly EndDate,
    VacationRequestStatus Status);