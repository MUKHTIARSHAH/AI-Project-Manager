using System.Net;
using FluentAssertions;

namespace AIPM.Host.Tests;

/// <summary>
/// Integration tests for host endpoints.
/// </summary>
public sealed class HealthEndpointTests : IClassFixture<HostTestWebApplicationFactory>
{
    private readonly HttpClient _client;

    /// <summary>Creates test client.</summary>
    public HealthEndpointTests(HostTestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>/health returns 200.</summary>
    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
    }

    /// <summary>/ready returns 200 including messaging health.</summary>
    [Fact]
    public async Task Ready_ReturnsOk()
    {
        var response = await _client.GetAsync("/ready");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
    }
}
