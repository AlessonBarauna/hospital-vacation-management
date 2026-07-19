using FluentValidation;

namespace HospitalVacationManagement.Application.Users;

public sealed class ChangeOwnPasswordRequestValidator : AbstractValidator<ChangeOwnPasswordRequest>
{
    public ChangeOwnPasswordRequestValidator()
    {
        RuleFor(request => request.CurrentPassword)
            .NotEmpty();

        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .Matches("[a-z]")
            .Matches("[0-9]")
            .Matches("[^a-zA-Z0-9]");
    }
}