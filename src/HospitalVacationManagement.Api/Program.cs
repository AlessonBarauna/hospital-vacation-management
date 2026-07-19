using HospitalVacationManagement.Application;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Infrastructure;
using HospitalVacationManagement.Domain.Vacations;
using FluentValidation;
using HospitalVacationManagement.Application.Common;
using Serilog;
using HospitalVacationManagement.Application.Authentication;
using HospitalVacationManagement.Application.Departments;
using HospitalVacationManagement.Application.Employees;
using HospitalVacationManagement.Application.System;
using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Application.Users;
using System.Security.Claims;
using HospitalVacationManagement.Api.Endpoints;
using HospitalVacationManagement.Api.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddApiSwagger();
builder.Services.AddApiHealthChecks(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddApiAuthorization();
var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapUserEndpoints();
app.MapMeEndpoints();
app.MapAuthEndpoints();
app.MapDepartmentEndpoints();
app.MapEmployeeEndpoints();
app.MapVacationRequestEndpoints();
app.MapSystemEndpoints();

app.MapGet("/health/live", () => Results.Ok("Healthy"))
    .WithName("Liveness")
    .WithOpenApi();

app.UseApiSwagger();

app.Run();

public partial class Program
{
}
