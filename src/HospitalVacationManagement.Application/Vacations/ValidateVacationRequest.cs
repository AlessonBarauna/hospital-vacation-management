namespace HospitalVacationManagement.Application.Vacations;

public sealed record ValidateVacationRequest(Guid EmployeeId, DateOnly StartDate, DateOnly EndDate);