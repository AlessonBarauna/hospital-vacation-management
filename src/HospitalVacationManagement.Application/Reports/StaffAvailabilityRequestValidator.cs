using FluentValidation;

namespace HospitalVacationManagement.Application.Reports;

public sealed class StaffAvailabilityRequestValidator : AbstractValidator<StaffAvailabilityRequest>
{
    public StaffAvailabilityRequestValidator()
    {
        RuleFor(request => request.DepartmentId)
            .NotEmpty();

        RuleFor(request => request.StartDate)
            .LessThanOrEqualTo(request => request.EndDate);

        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate);
    }
}