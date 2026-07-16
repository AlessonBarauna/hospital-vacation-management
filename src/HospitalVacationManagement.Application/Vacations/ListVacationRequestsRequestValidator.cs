using FluentValidation;

namespace HospitalVacationManagement.Application.Vacations;

public sealed class ListVacationRequestsRequestValidator : AbstractValidator<ListVacationRequestsRequest>
{
    public ListVacationRequestsRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than zero.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .When(request => request.StartDate.HasValue && request.EndDate.HasValue)
            .WithMessage("End date cannot be before start date.");
    }
}