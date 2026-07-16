using HospitalVacationManagement.Application;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Infrastructure;
using HospitalVacationManagement.Domain.Vacations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/vacation-requests", async (
    VacationRequestStatus? status,
    Guid? employeeId,
    Guid? departmentId,
    DateOnly? startDate,
    DateOnly? endDate,
    int page,
    int pageSize,
    ListVacationRequestsHandler handler,
    CancellationToken cancellationToken) =>
{
    var request = new ListVacationRequestsRequest(
        status,
        employeeId,
        departmentId,
        startDate,
        endDate,
        page,
        pageSize);

    var response = await handler.HandleAsync(request, cancellationToken);

    return Results.Ok(response);
})
.WithName("ListVacationRequests")
.WithOpenApi();

app.MapPost("/vacation-requests/validate", async (
    ValidateVacationRequest request,
    ValidateVacationRequestHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(request, cancellationToken);

    return response.IsValid
        ? Results.Ok(response)
        : Results.BadRequest(response);
})
.WithName("ValidateVacationRequest")
.WithOpenApi();

app.MapPut("/vacation-requests/{id:guid}/approve", async (
    Guid id,
    ApproveVacationRequestHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(id, cancellationToken);

    return response.IsSuccess
        ? Results.NoContent()
        : Results.BadRequest(response);
})
.WithName("ApproveVacationRequest")
.WithOpenApi();

app.MapPut("/vacation-requests/{id:guid}/reject", async (
    Guid id,
    RejectVacationRequestHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(id, cancellationToken);

    return response.IsSuccess
        ? Results.NoContent()
        : Results.BadRequest(response);
})
.WithName("RejectVacationRequest")
.WithOpenApi();

app.MapPut("/vacation-requests/{id:guid}/cancel", async (
    Guid id,
    CancelVacationRequestHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(id, cancellationToken);

    return response.IsSuccess
        ? Results.NoContent()
        : Results.BadRequest(response);
})
.WithName("CancelVacationRequest")
.WithOpenApi();

app.Run();