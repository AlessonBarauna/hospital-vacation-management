using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HospitalVacationManagement.Tests.Integration;

public sealed class CorrelationIdTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CorrelationIdTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetHealthLive_ShouldReturnCorrelationIdHeader()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.True(response.Headers.Contains("X-Correlation-Id"));
    }

    [Fact]
    public async Task GetHealthLive_ShouldReuseCorrelationId_WhenHeaderWasSent()
    {
        var client = _factory.CreateClient();

        var correlationId = Guid.NewGuid().ToString("N");

        client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);

        var response = await client.GetAsync("/health/live");

        Assert.True(response.Headers.Contains("X-Correlation-Id"));

        var responseCorrelationId = response.Headers.GetValues("X-Correlation-Id").Single();

        Assert.Equal(correlationId, responseCorrelationId);
    }
}