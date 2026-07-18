using FluentValidation;

namespace HospitalVacationManagement.Application.Users;

public sealed class ChangeUserPasswordRequestValidator : AbstractValidator<ChangeUserPasswordRequest>
{
    public ChangeUserPasswordRequestValidator()
    {
        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .Matches("[^a-zA-Z0-9]");
    }
}