using HospitalVacationManagement.Application.Calendar;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace HospitalVacationManagement.Api.Endpoints;

public static class CalendarEndpoints
{
    public static IEndpointRouteBuilder MapCalendarEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/calendar/vacations", async (
            Guid? departmentId,
            int year,
            int month,
            IValidator<ListVacationCalendarRequest> validator,
            [FromServices] ListVacationCalendarHandler handler,
            CancellationToken cancellationToken) =>
        {
            var request = new ListVacationCalendarRequest(
                    departmentId,
                    year,
                    month);
                    
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var response = await handler.HandleAsync(request, cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        return app;
    }
}