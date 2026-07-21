using HospitalVacationManagement.Application.Reports;
using FluentValidation;

namespace HospitalVacationManagement.Api.Endpoints;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reports/vacations-by-department", async (
            int year,
            int month,
            IValidator<VacationsByDepartmentRequest> validator,
            VacationsByDepartmentHandler handler,
            CancellationToken cancellationToken) =>
        {
            var request = new VacationsByDepartmentRequest(year, month);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var response = await handler.HandleAsync(request, cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization("ManagerOrAdmin");

        app.MapGet("/reports/staff-availability", async (
            Guid departmentId,
            DateOnly startDate,
            DateOnly endDate,
            IValidator<StaffAvailabilityRequest> validator,
            StaffAvailabilityHandler handler,
            CancellationToken cancellationToken) =>
    {
        var request = new StaffAvailabilityRequest(
            departmentId,
            startDate,
            endDate);

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await handler.xdxxxHandleAsync(request, cancellationToken);

        return Results.Ok(response);
    })
    .RequireAuthorization("ManagerOrAdmin");

        return app;
    }
}