using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HospitalVacationManagement.Application.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using HospitalVacationManagement.Domain.Departments;
using HospitalVacationManagement.Domain.Employees;
using HospitalVacationManagement.Domain.Users;
using HospitalVacationManagement.Domain.Vacations;
using HospitalVacationManagement.Infrastructure.Auth;
using HospitalVacationManagement.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalVacationManagement.Tests.Integration;

public sealed class VacationRequestAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public VacationRequestAuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Cancel_ShouldReturnForbidden_WhenUserIsNotOwnerOrManager()
    {
        var client = _factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            "admin@hospital.com",
            "Admin@123"));

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            loginContent!.AccessToken);

        var response = await client.PutAsync($"/api/v1/vacation-requests/{Guid.NewGuid()}/cancel", null);

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound,
            $"Expected Forbidden or NotFound, but got {(int)response.StatusCode} {response.StatusCode}. Body: {responseBody}");
    }

    [Fact]
    public async Task Cancel_ShouldReturnForbidden_WhenRegularUserCancelsAnotherEmployeeVacation()
    {
        var client = _factory.CreateClient();

        var testId = Guid.NewGuid();

        var userTryingToCancelId = Guid.NewGuid();
        var vacationOwnerUserId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var vacationOwnerEmployeeId = Guid.NewGuid();
        var vacationRequestId = Guid.NewGuid();

        var userTryingToCancelEmail = $"trying.user.{testId}@hospital.com";
        var vacationOwnerEmail = $"vacation.owner.{testId}@hospital.com";

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = new PasswordHasher();

        var userTryingToCancel = new User(
            userTryingToCancelId,
            "User Trying",
            userTryingToCancelEmail,
            passwordHasher.Hash("User@123"),
            UserRole.Employee);

        var vacationOwnerUser = new User(
            vacationOwnerUserId,
            "Vacation Owner",
            vacationOwnerEmail,
            passwordHasher.Hash("Owner@123"),
            UserRole.Employee);

        var department = new Department(
            departmentId,
            "Emergency",
            maximumSimultaneousVacations: 2);

        var vacationOwnerEmployee = new Employee(
            vacationOwnerEmployeeId,
            "Vacation Owner",
            departmentId,
            JobRole.Nurse,
            SeniorityLevel.Senior);

        var vacationRequest = new VacationRequest(
            vacationRequestId,
            vacationOwnerEmployeeId,
            departmentId,
            new DateOnly(2026, 8, 10),
            new DateOnly(2026, 8, 20),
            VacationRequestStatus.Pending,
            vacationOwnerUserId);

        dbContext.Users.AddRange(userTryingToCancel, vacationOwnerUser);
        dbContext.Departments.Add(department);
        dbContext.Employees.Add(vacationOwnerEmployee);
        dbContext.VacationRequests.Add(vacationRequest);

        await dbContext.SaveChangesAsync();

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(
            userTryingToCancelEmail,
            "User@123"));

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            loginContent!.AccessToken);

        var response = await client.PutAsync($"/api/v1/vacation-requests/{vacationRequestId}/cancel", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}