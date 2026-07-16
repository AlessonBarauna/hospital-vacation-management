using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Domain.Vacations;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalVacationManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IVacationPolicyService, VacationPolicyService>();
        services.AddScoped<ValidateVacationRequestHandler>();
        services.AddScoped<ListVacationRequestsHandler>();
        
        return services;
    }
}