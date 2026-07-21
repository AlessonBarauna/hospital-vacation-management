namespace HospitalVacationManagement.Api.Middlewares;
using Serilog.Context;

public sealed class CorrelationIdMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var correlationId = GetOrCreateCorrelationId(httpContext);

        httpContext.Response.Headers[CorrelationIdHeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(httpContext);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId)
            && !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId.ToString();
        }

        return Guid.NewGuid().ToString("N");
    }
}