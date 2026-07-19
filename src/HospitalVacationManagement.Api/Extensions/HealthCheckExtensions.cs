namespace HospitalVacationManagement.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApiHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Database connection string was not configured.");

        services.AddHealthChecks()
            .AddNpgSql(connectionString);

        return services;
    }
}