namespace HospitalVacationManagement.Domain.Vacations;

public sealed record VacationValidationResult(bool IsValid, IReadOnlyCollection<string> Errors)
{
    public static VacationValidationResult Success()
    {
        return new VacationValidationResult(true, Array.Empty<string>());
    }

    public static VacationValidationResult Failure(IReadOnlyCollection<string> errors)
    {
        return new VacationValidationResult(false, errors);
    }
}