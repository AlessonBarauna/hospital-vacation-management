using HospitalVacationManagement.Application;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Infrastructure;
using HospitalVacationManagement.Domain.Vacations;
using FluentValidation;
using HospitalVacationManagement.Application.Common;
using Serilog;
using System.Text;
using HospitalVacationManagement.Application.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using HospitalVacationManagement.Application.Departments;
using HospitalVacationManagement.Application.Employees;
using HospitalVacationManagement.Application.System;
using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Application.Users;
using System.Security.Claims;
using HospitalVacationManagement.Api.Endpoints;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Digite o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});
builder.Services
    .AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireRole("Manager", "Admin"));
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapUserEndpoints();
app.MapMeEndpoints();
app.MapAuthEndpoints();
app.MapDepartmentEndpoints();
app.MapEmployeeEndpoints();
app.MapVacationRequestEndpoints();

app.MapGet("/health/live", () => Results.Ok("Healthy"))
    .WithName("Liveness")
    .WithOpenApi();

app.MapGet("/version", (IHostEnvironment environment) =>
{
    var response = new VersionResponse(
        "Hospital Vacation Management API",
        environment.EnvironmentName,
        "1.0.0",
        DateTime.UtcNow);

    return Results.Ok(response);
})
.WithName("GetVersion")
.WithOpenApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public partial class Program
{
}
