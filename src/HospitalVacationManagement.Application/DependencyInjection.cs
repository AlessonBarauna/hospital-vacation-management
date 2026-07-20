using HospitalVacationManagement.Application.Vacations;
using HospitalVacationManagement.Domain.Vacations;
using Microsoft.Extensions.DependencyInjection;
using HospitalVacationManagement.Application.Authentication;
using HospitalVacationManagement.Application.Departments;
using HospitalVacationManagement.Application.Employees;
using FluentValidation;
using HospitalVacationManagement.Application.Users;
using System.ComponentModel;
using HospitalVacationManagement.Application.Calendar;
using HospitalVacationManagement.Application.Reports;

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
        services.AddScoped<RequestVacationHandler>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<CreateDepartmentHandler>();
        services.AddScoped<ListDepartmentsHandler>();
        services.AddScoped<GetDepartmentByIdHandler>();
        services.AddScoped<CreateEmployeeHandler>();
        services.AddScoped<ListEmployeesHandler>();
        services.AddScoped<GetEmployeeByIdHandler>();
        services.AddScoped<ListEmployeesByDepartmentHandler>();
        services.AddScoped<GetVacationRequestByIdHandler>();
        services.AddScoped<CreateUserHandler>();
        services.AddScoped<ListUsersHandler>();
        services.AddScoped<GetUserByIdHandler>();
        services.AddScoped<UpdateUserHandler>();
        services.AddScoped<ChangeUserPasswordHandler>();
        services.AddScoped<DeactivateUserHandler>();
        services.AddScoped<ActivateUserHandler>();
        services.AddScoped<GetCurrentUserHandler>();
        services.AddScoped<ChangeOwnPasswordHandler>();
        services.AddScoped<ListChangedEventHandler>();
        services.AddScoped<ListVacationCalendarHandler>();
        services.AddScoped<VacationsByDepartmentHandler>();

        return services;
    }
}