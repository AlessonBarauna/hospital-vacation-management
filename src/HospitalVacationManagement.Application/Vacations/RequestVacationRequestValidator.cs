using FluentValidation;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class RequestVacationRequestValidator : AbstractValidator<RequestVacationRequest>
{
    public RequestVacationRequestValidator()
    {
        RuleFor(request => request.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee id is required.");

        RuleFor(request => request.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(request => request.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.");

        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .WithMessage("End date cannot be before start date.");
    }
}