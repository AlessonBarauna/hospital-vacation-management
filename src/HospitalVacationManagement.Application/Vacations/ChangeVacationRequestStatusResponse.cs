namespace HospitalVacationManagement.Application.Vacations;

public sealed record ChangeVacationRequestStatusResponse(
    bool IsSuccess,
    IReadOnlyCollection<string> Errors);