namespace HospitalVacationManagement.Application.Vacations;

public sealed record ValidateVacationResponse(bool IsValid, IReadOnlyCollection<string> Errors);