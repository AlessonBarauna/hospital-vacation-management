using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalVacationManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryDatabase>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IVacationRequestRepository, VacationRequestRepository>();

        return services;
    }
}