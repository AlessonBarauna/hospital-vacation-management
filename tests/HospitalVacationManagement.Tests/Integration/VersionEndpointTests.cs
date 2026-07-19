using System.Net.Http.Json;
using Xunit;

namespace HospitalVacationManagement.Tests.Integration;

public sealed class VersionEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VersionEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetVersion_ShouldReturnApplicationVersion()
    {
        var response = await _client.GetFromJsonAsync<VersionResponse>("/version");

        Assert.NotNull(response);
        Assert.Equal("Hospital Vacation Management API", response.Application);
        Assert.Equal("1.0.0", response.Version);
        Assert.Equal("Testing", response.Environment);
    }

    private sealed record VersionResponse(
        string Application,
        string Version,
        string Environment);
}