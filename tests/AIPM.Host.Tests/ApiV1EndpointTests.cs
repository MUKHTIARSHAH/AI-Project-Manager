using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AIPM.Host.Tests;

/// <summary>
/// Integration tests for versioned API routes.
/// </summary>
public sealed class ApiV1EndpointTests : IClassFixture<HostTestWebApplicationFactory>
{
    private readonly HttpClient _client;

    /// <summary>Creates test client.</summary>
    public ApiV1EndpointTests(HostTestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>/api/v1/platform/ping returns 200.</summary>
    [Fact]
    public async Task PlatformPing_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/v1/platform/ping");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>/api/v1/agent-types returns discovered catalog.</summary>
    [Fact]
    public async Task AgentTypes_ReturnsCatalog()
    {
        var response = await _client.GetAsync("/api/v1/agent-types");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
        body.Should().Contain("echo-agent");
        body.Should().Contain("schemaVersion");
    }

    /// <summary>/api/v1/ai/providers returns abstraction foundation status.</summary>
    [Fact]
    public async Task AiProviders_ReturnsStubProvider()
    {
        var response = await _client.GetAsync("/api/v1/ai/providers");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
        body.Should().Contain("stub");
        body.Should().Contain("abstraction-only");
    }

    [Fact]
    public async Task IdentityEndpoints_FailClosed_WithoutApiKey()
    {
        var response = await _client.GetAsync("/api/v1/identity/tenants");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task IdentityTenantWorkflow_Works_WithAuthorizedClient()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/tenants")
        {
            Content = JsonContent.Create(new { name = "Acme" })
        };
        request.Headers.Add("X-Api-Key", "test-bc10-key");
        var createTenantResponse = await _client.SendAsync(request);
        var payload = await createTenantResponse.Content.ReadAsStringAsync();
        createTenantResponse.StatusCode.Should().Be(HttpStatusCode.OK, because: payload);
        payload.Should().Contain("Acme");

        using var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/identity/tenants");
        listRequest.Headers.Add("X-Api-Key", "test-bc10-key");
        var listResponse = await _client.SendAsync(listRequest);
        var listBody = await listResponse.Content.ReadAsStringAsync();
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK, because: listBody);
        listBody.Should().Contain("Acme");
    }
}
