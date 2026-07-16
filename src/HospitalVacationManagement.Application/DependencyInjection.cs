using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Domain.Vacations;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace HospitalVacationManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IVacationPolicyService, VacationPolicyService>();
        services.AddScoped<ValidateVacationRequestHandler>();
        services.AddScoped<ListVacationRequestsHandler>();
        services.AddScoped<ApproveVacationRequestHandler>();
        services.AddScoped<RejectVacationRequestHandler>();
        services.AddScoped<CancelVacationRequestHandler>();

        return services;
    }
}