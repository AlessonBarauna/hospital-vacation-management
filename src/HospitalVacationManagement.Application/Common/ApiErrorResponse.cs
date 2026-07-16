namespace HospitalVacationManagement.Application.Common;

public sealed record ApiErrorResponse(IReadOnlyCollection<string> Errors);