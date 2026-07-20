using FluentValidation;

namespace HospitalVacationManagement.Application.Calendar;

public sealed class ListVacationCalendarRequestValidator : AbstractValidator<ListVacationCalendarRequest>
{
    public ListVacationCalendarRequestValidator()
    {
        RuleFor(request => request.Year)
            .InclusiveBetween(2000, 2100);

        RuleFor(request => request.Month)
            .InclusiveBetween(1, 12);
    }
}