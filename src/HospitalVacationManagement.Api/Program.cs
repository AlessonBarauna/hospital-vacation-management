using HospitalVacationManagement.Application;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Infrastructure;

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

app.MapGet("/vacation-requests", async (
    ListVacationRequestsHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(cancellationToken);

    return Results.Ok(response);
})
.WithName("ListVacationRequests")
.WithOpenApi();

app.Run();