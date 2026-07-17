using FluentValidation;

namespace HospitalVacationManagement.Application.Departments;

public sealed class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(100)
            .WithMessage("Department name must have at most 100 characters.");

        RuleFor(request => request.MaximumSimultaneousVacations)
            .GreaterThan(0)
            .WithMessage("Maximum simultaneous vacations must be greater than zero.");
    }
}