using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace AIPM.Host.Tests;

/// <summary>
/// RFC 7807 Problem Details integration tests.
/// </summary>
public sealed class ProblemDetailsTests : IClassFixture<HostTestWebApplicationFactory>
{
    private readonly HttpClient _client;

    /// <summary>Creates test client.</summary>
    public ProblemDetailsTests(HostTestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>Missing agent capability returns application/problem+json 404.</summary>
    [Fact]
    public async Task UnknownAgentCapability_ReturnsProblemDetails404()
    {
        var response = await _client.GetAsync("/api/v1/agents/unknown-capability");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
        problem.Title.Should().Be("Not Found");
        problem.Type.Should().StartWith("https://aipm.dev/problems/");
        problem.Extensions.Should().ContainKey("correlationId");
    }

    /// <summary>Deployment endpoint returns active profile.</summary>
    [Fact]
    public async Task DeploymentEndpoint_ReturnsProfile()
    {
        var response = await _client.GetAsync("/api/v1/platform/deployment");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("saas");
    }
}
