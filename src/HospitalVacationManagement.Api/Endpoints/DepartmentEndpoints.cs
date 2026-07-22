using HospitalVacationManagement.Application.Departments;
using Microsoft.AspNetCore.Mvc;
using HospitalVacationManagement.Api.Errors;

namespace HospitalVacationManagement.Api.Endpoints;

public static class DepartmentEndpoints
{
    public static IEndpointRouteBuilder MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/departments", async (
            CreateDepartmentRequest request,
            [FromServices] CreateDepartmentHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(request, cancellationToken);

            return Results.Created($"/departments/{response.Id}", response);
        })
        .RequireAuthorization("AdminOnly");

        app.MapGet("/departments", async (
            [FromServices] ListDepartmentsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapGet("/departments/{id:guid}", async (
            Guid id,
            [FromServices] GetDepartmentByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(id, cancellationToken);

            return response is null
                ? ApiErrors.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization();

        return app;
    }
}