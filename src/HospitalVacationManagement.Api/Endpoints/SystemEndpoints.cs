namespace HospitalVacationManagement.Api.Endpoints;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/version", (IHostEnvironment environment) =>
        {
            return Results.Ok(new
            {
                Application = "Hospital Vacation Management API",
                Version = "1.0.0",
                Environment = environment.EnvironmentName
            });
        });

        app.MapHealthChecks("/health");

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false
        });

        return app;
    }
}