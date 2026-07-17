using System.Net.Http.Json;
using HospitalVacationManagement.Application.System;
using Xunit;

namespace HospitalVacationManagement.Tests.Integration;

public sealed class VersionEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public VersionEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetVersion_ShouldReturnApplicationVersion()
    {
        var client = _factory.CreateClient();

        var response = await client.GetFromJsonAsync<VersionResponse>("/version");

        Assert.NotNull(response);
        Assert.Equal("Hospital Vacation Management API", response.Application);
        Assert.Equal("Testing", response.Environment);
        Assert.False(string.IsNullOrWhiteSpace(response.Version));
    }
}