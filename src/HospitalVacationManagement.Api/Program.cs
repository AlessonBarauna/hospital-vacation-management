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

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/auth/login", (
    LoginRequest request,
    IJwtTokenGenerator jwtTokenGenerator) =>
{
    if (!string.Equals(request.Email, "admin@hospital.com", StringComparison.OrdinalIgnoreCase)
        || request.Password != "Admin@123")
    {
        return Results.Unauthorized();
    }

    var response = jwtTokenGenerator.Generate(request.Email, "Admin");

    return Results.Ok(response);
})
.WithName("Login")
.WithOpenApi();

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
.RequireAuthorization();

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
.RequireAuthorization();

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
.RequireAuthorization();

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
.RequireAuthorization();

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

app.Run();
