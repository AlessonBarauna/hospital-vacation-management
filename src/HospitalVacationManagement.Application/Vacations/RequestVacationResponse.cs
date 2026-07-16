namespace HospitalVacationManagement.Application.Vacations;

public sealed record RequestVacationResponse(
    bool IsValid,
    Guid? VacationRequestId,
    IReadOnlyCollection<string> Errors);