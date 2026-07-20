using HospitalVacationManagement.Application.Calendar;

namespace HospitalVacationManagement.Api.Endpoints;

public static class CalendarEndpoints
{
    public static IEndpointRouteBuilder MapCalendarEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/calendar/vacations", async (
            Guid? departmentId,
            int year,
            int month,
            ListVacationCalendarHandler handler,
            CancellationToken cancellationToken) =>
        {
            var request = new ListVacationCalendarRequest(
                departmentId,
                year,
                month);

            var response = await handler.HandleAsync(request, cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        return app;
    }
}