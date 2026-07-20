namespace HospitalVacationManagement.Application.Calendar;

public sealed record ListVacationCalendarRequest(
    Guid? DepartmentId,
    int Year,
    int Month);