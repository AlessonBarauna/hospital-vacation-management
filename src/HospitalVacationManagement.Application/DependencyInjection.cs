using FluentValidation;
using HospitalVacationManagement.Application.Authentication;
using HospitalVacationManagement.Application.Calendar;
using HospitalVacationManagement.Application.Departments;
using HospitalVacationManagement.Application.Employees;
using HospitalVacationManagement.Application.Reports;
using HospitalVacationManagement.Application.Users;
using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Domain.Vacations;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalVacationManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IVacationPolicyService, VacationPolicyService>();

        services.AddScoped<CreateDepartmentHandler>();
        services.AddScoped<ListDepartmentsHandler>();
        services.AddScoped<GetDepartmentByIdHandler>();

        services.AddScoped<CreateEmployeeHandler>();
        services.AddScoped<ListEmployeesHandler>();
        services.AddScoped<GetEmployeeByIdHandler>();
        services.AddScoped<ListEmployeesByDepartmentHandler>();

        services.AddScoped<RequestVacationHandler>();
        services.AddScoped<ValidateVacationRequestHandler>();
        services.AddScoped<ListVacationRequestsHandler>();
        services.AddScoped<GetVacationRequestByIdHandler>();
        services.AddScoped<ApproveVacationRequestHandler>();
        services.AddScoped<RejectVacationRequestHandler>();
        services.AddScoped<CancelVacationRequestHandler>();

        services.AddScoped<ListVacationCalendarHandler>();

        services.AddScoped<VacationsByDepartmentHandler>();
        services.AddScoped<StaffAvailabilityHandler>();

        services.AddScoped<CreateUserHandler>();
        services.AddScoped<ListUsersHandler>();
        services.AddScoped<GetUserByIdHandler>();
        services.AddScoped<UpdateUserHandler>();
        services.AddScoped<ChangeUserPasswordHandler>();
        services.AddScoped<ChangeOwnPasswordHandler>();
        services.AddScoped<ActivateUserHandler>();
        services.AddScoped<DeactivateUserHandler>();
        services.AddScoped<GetCurrentUserHandler>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}