using HospitalVacationManagement.Application;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Infrastructure;
using HospitalVacationManagement.Domain.Vacations;
using FluentValidation;
using HospitalVacationManagement.Application.Common;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.MapHealthChecks("/health");

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
    IValidator<ListVacationRequestsRequest> validator,
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

    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(new ApiErrorResponse(
            validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
    }

    var response = await handler.HandleAsync(request, cancellationToken);

    return Results.Ok(response);
})
.WithName("ListVacationRequests")
.WithOpenApi();

app.MapPost("/vacation-requests", async (
    RequestVacationRequest request,
    IValidator<RequestVacationRequest> validator,
    RequestVacationHandler handler,
    CancellationToken cancellationToken) =>
{
    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(new ApiErrorResponse(
    validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
    }

    var response = await handler.HandleAsync(request, cancellationToken);

    return response.IsValid
        ? Results.Created($"/vacation-requests/{response.VacationRequestId}", response)
        : Results.BadRequest(response);
})
.WithName("RequestVacation")
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
