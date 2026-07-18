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

app.MapPost("/auth/login", async (
    LoginRequest request,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    CancellationToken cancellationToken) =>
{
    var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

    if (user is null || !user.IsActive)
    {
        return Results.Unauthorized();
    }

    var passwordIsValid = passwordHasher.Verify(request.Password, user.PasswordHash);

    if (!passwordIsValid)
    {
        return Results.Unauthorized();
    }

    var loginResponse = jwtTokenGenerator.Generate(
    user.Id,
    user.Email,
    user.Role.ToString());

    return Results.Ok(loginResponse);
});

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
.WithOpenApi()
.RequireAuthorization();

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
.WithOpenApi()
.RequireAuthorization();

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
.WithOpenApi()
.RequireAuthorization("ManagerOrAdmin");

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
.WithOpenApi()
.RequireAuthorization("ManagerOrAdmin");

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
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/departments", async (
    CreateDepartmentRequest request,
    IValidator<CreateDepartmentRequest> validator,
    CreateDepartmentHandler handler,
    CancellationToken cancellationToken) =>
{
    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(new ApiErrorResponse(
            validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
    }

    var response = await handler.HandleAsync(request, cancellationToken);

    return Results.Created($"/departments/{response.Id}", response);
})
.WithName("CreateDepartment")
.WithOpenApi()
.RequireAuthorization("AdminOnly");

app.MapGet("/departments", async (
    ListDepartmentsHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(cancellationToken);

    return Results.Ok(response);
})
.WithName("ListDepartments")
.WithOpenApi();

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
.WithName("GetDepartmentById")
.WithOpenApi();

app.MapPost("/employees", async (
    CreateEmployeeRequest request,
    IValidator<CreateEmployeeRequest> validator,
    CreateEmployeeHandler handler,
    CancellationToken cancellationToken) =>
{
    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(new ApiErrorResponse(
            validationResult.Errors.Select(error => error.ErrorMessage).ToList()));
    }

    var response = await handler.HandleAsync(request, cancellationToken);

    return response is null
        ? Results.BadRequest(new ApiErrorResponse(["Department was not found."]))
        : Results.Created($"/employees/{response.Id}", response);
})
.WithName("CreateEmployee")
.WithOpenApi()
.RequireAuthorization("AdminOnly");

app.MapGet("/employees", async (
    ListEmployeesHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(cancellationToken);

    return Results.Ok(response);
})
.WithName("ListEmployees")
.WithOpenApi();

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
.WithName("GetEmployeeById")
.WithOpenApi();

app.MapGet("/departments/{departmentId:guid}/employees", async (
    Guid departmentId,
    ListEmployeesByDepartmentHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(departmentId, cancellationToken);

    return Results.Ok(response);
})
.WithName("ListEmployeesByDepartment")
.WithOpenApi();

app.MapGet("/vacation-requests/{id:guid}", async (
    Guid id,
    GetVacationRequestByIdHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(id, cancellationToken);

    return response is null
        ? Results.NotFound()
        : Results.Ok(response);
})
.WithName("GetVacationRequestById")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/users", async (
    CreateUserRequest request,
    IValidator<CreateUserRequest> validator,
    CreateUserHandler handler,
    CancellationToken cancellationToken) =>
{
    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var response = await handler.HandleAsync(request, cancellationToken);

    return Results.Created($"/users/{response.Id}", response);
})
.RequireAuthorization("AdminOnly");

app.MapGet("/users", async (
    ListUsersHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(cancellationToken);

    return Results.Ok(response);
})
.RequireAuthorization("AdminOnly");

app.MapGet("/users/{id:guid}", async (
    Guid id,
    GetUserByIdHandler handler,
    CancellationToken cancellationToken) =>
{
    var response = await handler.HandleAsync(id, cancellationToken);

    return response is null
        ? Results.NotFound()
        : Results.Ok(response);
})
.RequireAuthorization("AdminOnly");

app.MapPut("/users/{id:guid}", async (
    Guid id,
    UpdateUserRequest request,
    IValidator<UpdateUserRequest> validator,
    UpdateUserHandler handler,
    CancellationToken cancellationToken) =>
{
    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var response = await handler.HandleAsync(id, request, cancellationToken);

    return response is null
        ? Results.NotFound()
        : Results.Ok(response);
})
.RequireAuthorization("AdminOnly");

app.MapPut("/users/{id:guid}/password", async (
    Guid id,
    ChangeUserPasswordRequest request,
    IValidator<ChangeUserPasswordRequest> validator,
    ChangeUserPasswordHandler handler,
    CancellationToken cancellationToken) =>
{
    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var passwordWasChanged = await handler.HandleAsync(id, request, cancellationToken);

    return passwordWasChanged
        ? Results.NoContent()
        : Results.NotFound();
})
.RequireAuthorization("AdminOnly");

app.MapPut("/users/{id:guid}/deactivate", async (
    Guid id,
    ClaimsPrincipal currentUser,
    GetUserByIdHandler getUserByIdHandler,
    DeactivateUserHandler deactivateUserHandler,
    CancellationToken cancellationToken) =>
{
    var userToDeactivate = await getUserByIdHandler.HandleAsync(id, cancellationToken);

    if (userToDeactivate is null)
    {
        return Results.NotFound();
    }

    var currentUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

    if (currentUserId == id.ToString())
    {
        return Results.BadRequest("You cannot deactivate your own user.");
    }

    var userWasDeactivated = await deactivateUserHandler.HandleAsync(id, cancellationToken);

    return userWasDeactivated
        ? Results.NoContent()
        : Results.NotFound();
})
.RequireAuthorization("AdminOnly");

app.MapPut("/users/{id:guid}/activate", async (
    Guid id,
    ActivateUserHandler handler,
    CancellationToken cancellationToken) =>
{
    var userWasActivated = await handler.HandleAsync(id, cancellationToken);

    return userWasActivated
        ? Results.NoContent()
        : Results.NotFound();
})
.RequireAuthorization("AdminOnly");

app.Run();

public partial class Program
{
}
