using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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

    [Fact]
    public async Task IdentityEndpoints_RejectInvalidApiKey()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/identity/tenants");
        request.Headers.Add("X-Api-Key", "invalid-key");
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SuspendTenant_ReturnsNoContent()
    {
        var tenantId = await ProvisionTenantAsync("Suspend-Me");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/identity/tenants/{tenantId}/suspend");
        request.Headers.Add("X-Api-Key", "test-bc10-key");
        request.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task SuspendTenant_WithoutTenantHeader_AllowsPlatformAdmin()
    {
        var tenantId = await ProvisionTenantAsync("Platform-Suspend");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/identity/tenants/{tenantId}/suspend");
        request.Headers.Add("X-Api-Key", "test-bc10-key");
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task CreateUserAndRole_Works_WithTenantHeader()
    {
        var tenantId = await ProvisionTenantAsync("Role-Tenant");

        using var roleRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/roles")
        {
            Content = JsonContent.Create(new { tenantId, name = "Admin" })
        };
        roleRequest.Headers.Add("X-Api-Key", "test-bc10-key");
        roleRequest.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var roleResponse = await _client.SendAsync(roleRequest);
        roleResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var userRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/users")
        {
            Content = JsonContent.Create(new { tenantId, email = "admin@role-tenant.test" })
        };
        userRequest.Headers.Add("X-Api-Key", "test-bc10-key");
        userRequest.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var userResponse = await _client.SendAsync(userRequest);
        userResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<Guid> ProvisionTenantAsync(string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/tenants")
        {
            Content = JsonContent.Create(new { name })
        };
        request.Headers.Add("X-Api-Key", "test-bc10-key");
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var document = await response.Content.ReadFromJsonAsync<JsonElement>();
        return document.GetProperty("id").GetGuid();
    }
}
