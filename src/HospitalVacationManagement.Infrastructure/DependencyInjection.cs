using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Infrastructure.Data;
using HospitalVacationManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace HospitalVacationManagement.Infrastructure;
using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Infrastructure.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
            

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IVacationRequestRepository, VacationRequestRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddHostedService<DatabaseMigrationService>();
        services.AddHostedService<DatabaseSeedService>();
        

        return services;
    }
}