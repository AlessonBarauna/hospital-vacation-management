using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Application.Authentication;
using HospitalVacationManagement.Domain.Users;
using HospitalVacationManagement.Tests.Fakes;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace HospitalVacationManagement.Tests.Integration;

public sealed class AuthenticationEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthenticationEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMe_ShouldReturnUnauthorized_WhenTokenWasNotProvided()
    {
        var response = await _client.GetAsync("/api/v1/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IUserRepository, FakeUserRepository>();
            });
        })
        .CreateClient();

        var request = new
        {
            Email = "usuario.inexistente@hospital.com",
            Password = "SenhaErrada@123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUnauthorized_WhenTokenWasNotProvided()
    {
        var response = await _client.GetAsync("/api/v1/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        var tokenGenerator = _factory.Services.GetRequiredService<IJwtTokenGenerator>();

        var loginResponse = tokenGenerator.Generate(
            Guid.NewGuid(),
            "employee@hospital.com",
            UserRole.Employee.ToString());

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            loginResponse.AccessToken);

        var response = await client.GetAsync("/api/v1/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
{
    var userRepository = new FakeUserRepository();

    var passwordHasher = _factory.Services.GetRequiredService<IPasswordHasher>();

    var user = new User(
        Guid.NewGuid(),
        "Maria Gestora",
        "maria.gestora@hospital.com",
        passwordHasher.Hash("Maria@123"),
        UserRole.Manager);

    await userRepository.AddAsync(user);

    var client = _factory.WithWebHostBuilder(builder =>
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<IUserRepository>(_ => userRepository);
        });
    })
    .CreateClient();

    var request = new
    {
        Email = "maria.gestora@hospital.com",
        Password = "Maria@123"
    };

    var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

    Assert.NotNull(loginResponse);
    Assert.False(string.IsNullOrWhiteSpace(loginResponse.AccessToken));
    }
}