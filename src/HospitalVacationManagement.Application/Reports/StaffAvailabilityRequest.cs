namespace HospitalVacationManagement.Application.Reports;

public sealed record StaffAvailabilityRequest(
    Guid DepartmentId,
    DateOnly StartDate,
    DateOnly EndDate);