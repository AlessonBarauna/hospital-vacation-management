using HospitalVacationManagement.Application.Reports;

namespace HospitalVacationManagement.Api.Endpoints;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reports/vacations-by-department", async (
            int year,
            int month,
            VacationsByDepartmentHandler handler,
            CancellationToken cancellationToken) =>
        {
            var request = new VacationsByDepartmentRequest(year, month);

            var response = await handler.HandleAsync(request, cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization("ManagerOrAdmin");

        return app;
    }
}