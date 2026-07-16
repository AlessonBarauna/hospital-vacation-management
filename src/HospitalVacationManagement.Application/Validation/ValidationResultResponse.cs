namespace HospitalVacationManagement.Application.Validation;

public sealed record ValidationResultResponse(IReadOnlyCollection<string> Errors)
{
    public bool IsValid => Errors.Count ==0;
}