namespace HospitalVacationManagement.Application.Vacations;

public sealed record RequestVacationRequest(Guid EmployeeId, DateOnly StartDate, DateOnly EndDate);