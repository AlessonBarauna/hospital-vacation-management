using FluentValidation;

namespace HospitalVacationManagement.Application.Employees;

public sealed class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(request => request.FullName)
            .NotEmpty()
            .WithMessage("Employee full name is required.")
            .MaximumLength(150)
            .WithMessage("Employee full name must have at most 150 characters.");

        RuleFor(request => request.DepartmentId)
            .NotEmpty()
            .WithMessage("Department id is required.");

        RuleFor(request => request.Role)
            .IsInEnum()
            .WithMessage("Employee role is invalid.");

        RuleFor(request => request.SeniorityLevel)
            .IsInEnum()
            .WithMessage("Employee seniority level is invalid.");
    }
}