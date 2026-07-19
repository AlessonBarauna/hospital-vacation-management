using HospitalVacationManagement.Application.Departments;

namespace HospitalVacationManagement.Api.Endpoints;

public static class DepartmentEndpoints
{
    public static IEndpointRouteBuilder MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/departments", async (
            CreateDepartmentRequest request,
            CreateDepartmentHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(request, cancellationToken);

            return Results.Created($"/departments/{response.Id}", response);
        })
        .RequireAuthorization("AdminOnly");

        app.MapGet("/departments", async (
            ListDepartmentsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapGet("/departments/{id:guid}", async (
            Guid id,
            GetDepartmentByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(id, cancellationToken);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization();

        return app;
    }
}