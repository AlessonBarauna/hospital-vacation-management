using System.Net;
using Xunit;
using System.Net.Http.Json;

namespace HospitalVacationManagement.Tests.Integration;

public sealed class AuthenticationEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthenticationEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMe_ShouldReturnUnauthorized_WhenTokenWasNotProvided()
    {
        var response = await _client.GetAsync("/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        var request = new
        {
            Email = "usuario.inexistente@hospital.com",
            Password = "SenhaErrada@123"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}