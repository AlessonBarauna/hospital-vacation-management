using FluentValidation;

namespace HospitalVacationManagement.Application.Reports;

public sealed class VacationsByDepartmentRequestValidator : AbstractValidator<VacationsByDepartmentRequest>
{
    public VacationsByDepartmentRequestValidator()
    {
        RuleFor(request => request.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(request => request.Month)
            .InclusiveBetween(1, 12);
    }
}