using HospitalVacationManagement.Application.Employees;
using Microsoft.AspNetCore.Mvc;

namespace HospitalVacationManagement.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/employees", async (
            CreateEmployeeRequest request,
            [FromServices] CreateEmployeeHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(request, cancellationToken);

            return response is null
        ? Results.BadRequest("Department was not found.")
        : Results.Created($"/employees/{response.Id}", response);
})
.RequireAuthorization("AdminOnly");

        app.MapGet("/employees", async (
            [FromServices] ListEmployeesHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapGet("/employees/{id:guid}", async (
            Guid id,
            [FromServices] GetEmployeeByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(id, cancellationToken);

            return response is null
                ? Results.NotFound()
                : Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapGet("/departments/{departmentId:guid}/employees", async (
            Guid departmentId,
            [FromServices] ListEmployeesByDepartmentHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(departmentId, cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        return app;
    }
}