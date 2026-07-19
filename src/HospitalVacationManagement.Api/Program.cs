using HospitalVacationManagement.Application;
using HospitalVacationManagement.Infrastructure;
using FluentValidation;
using Serilog;
using HospitalVacationManagement.Application.Authentication;
using HospitalVacationManagement.Api.Endpoints;
using HospitalVacationManagement.Api.Extensions;
using HospitalVacationManagement.Api.Middlewares;

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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthEndpoints();
app.MapApiEndpoints();

app.UseApiSwagger();

app.Run();

public partial class Program
{
}
