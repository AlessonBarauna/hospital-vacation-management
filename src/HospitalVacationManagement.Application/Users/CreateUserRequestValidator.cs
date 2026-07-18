using FluentValidation;
using HospitalVacationManagement.Domain.Users;

namespace HospitalVacationManagement.Application.Users;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .Matches("[^a-zA-Z0-9]");

        RuleFor(request => request.Role)
            .IsInEnum()
            .Must(role => role != 0);
    }
}