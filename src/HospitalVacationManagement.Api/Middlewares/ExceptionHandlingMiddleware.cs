using Microsoft.AspNetCore.Mvc;

namespace HospitalVacationManagement.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogWarning(
                exception,
                "A business rule violation occurred.");

            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Business rule violation",
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred.");

            var detail = _environment.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred.";

            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                detail);
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        if (httpContext.Response.HasStarted)
        {
            return;
        }

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}",
            Instance = httpContext.Request.Path
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}