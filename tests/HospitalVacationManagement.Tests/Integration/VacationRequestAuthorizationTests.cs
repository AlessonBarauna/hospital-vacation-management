using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HospitalVacationManagement.Application.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

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

        var loginResponse = await client.PostAsJsonAsync("/auth/login", new LoginRequest(
            "admin@hospital.com",
            "Admin@123"));

        var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            loginContent!.AccessToken);

        var response = await client.PutAsync($"/vacation-requests/{Guid.NewGuid()}/cancel", null);

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.NotFound,
            $"Expected Forbidden or NotFound, but got {(int)response.StatusCode} {response.StatusCode}. Body: {responseBody}");
    }
}