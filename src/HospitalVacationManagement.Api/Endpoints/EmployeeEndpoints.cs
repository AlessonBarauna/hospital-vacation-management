using HospitalVacationManagement.Application.Employees;

namespace HospitalVacationManagement.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/employees", async (
            CreateEmployeeRequest request,
            CreateEmployeeHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(request, cancellationToken);

            return response is null
        ? Results.BadRequest("Department was not found.")
        : Results.Created($"/employees/{response.Id}", response);
})
.RequireAuthorization("AdminOnly");

        app.MapGet("/employees", async (
            ListEmployeesHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        app.MapGet("/employees/{id:guid}", async (
            Guid id,
            GetEmployeeByIdHandler handler,
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
            ListEmployeesByDepartmentHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(departmentId, cancellationToken);

            return Results.Ok(response);
        })
        .RequireAuthorization();

        return app;
    }
}