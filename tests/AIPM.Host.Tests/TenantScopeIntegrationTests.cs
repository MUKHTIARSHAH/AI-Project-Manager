using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AIPM.Host.Tests;

/// <summary>
/// Integration tests for tenant-scoped execution context (H-02 + fail-closed hardening).
/// </summary>
public sealed class TenantScopeIntegrationTests
{
    private const string _apiKey = "test-bc10-key";

    /// <summary>Missing tenant header returns 400 when RequireTenantHeader is enabled.</summary>
    [Fact]
    public async Task RequireTenantHeader_WhenEnabled_ReturnsBadRequestWithoutHeader()
    {
        await using var factory = new TenantHeaderRequiredWebApplicationFactory();
        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/identity/users");
        request.Headers.Add("X-Api-Key", _apiKey);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("X-Tenant-Id");
    }

    /// <summary>Tenant-scoped mutations fail closed without tenant header.</summary>
    [Fact]
    public async Task CreateUser_WithoutTenantHeader_ReturnsBadRequest()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "No-Header-Tenant");

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/users")
        {
            Content = JsonContent.Create(new { tenantId, email = "user@no-header.test" })
        };
        request.Headers.Add("X-Api-Key", _apiKey);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>Tenant-scoped list operations fail closed without tenant header.</summary>
    [Fact]
    public async Task ListUsers_WithoutTenantHeader_ReturnsBadRequest()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/identity/users");
        request.Headers.Add("X-Api-Key", _apiKey);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>Platform administrator can list all tenants without tenant header.</summary>
    [Fact]
    public async Task PlatformAdmin_CanListTenants_WithoutTenantHeader()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        await ProvisionTenantAsync(client, "Platform-Tenant");

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/identity/tenants");
        request.Headers.Add("X-Api-Key", _apiKey);
        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
        body.Should().Contain("Platform-Tenant");
    }

    /// <summary>Platform administrator can suspend a tenant without tenant header.</summary>
    [Fact]
    public async Task PlatformAdmin_CanSuspendTenant_WithoutTenantHeader()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "Suspend-Platform");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/identity/tenants/{tenantId}/suspend");
        request.Headers.Add("X-Api-Key", _apiKey);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    /// <summary>Tenant administrator suspend requires matching tenant header.</summary>
    [Fact]
    public async Task SuspendTenant_WithMismatchedTenantHeader_ReturnsForbidden()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Tenant-A");
        var tenantB = await ProvisionTenantAsync(client, "Tenant-B");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/identity/tenants/{tenantB}/suspend");
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Tenant-Id", tenantA.ToString());
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>List users returns only users for the scoped tenant.</summary>
    [Fact]
    public async Task ListUsers_FiltersByTenantHeader()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Tenant-A");
        var tenantB = await ProvisionTenantAsync(client, "Tenant-B");

        await CreateUserAsync(client, tenantA, "user-a@tenant-a.test");
        await CreateUserAsync(client, tenantB, "user-b@tenant-b.test");

        var usersForA = await ListUsersAsync(client, tenantA);
        usersForA.Should().ContainSingle();
        usersForA[0].GetProperty("email").GetString().Should().Be("user-a@tenant-a.test");

        var usersForB = await ListUsersAsync(client, tenantB);
        usersForB.Should().ContainSingle();
        usersForB[0].GetProperty("email").GetString().Should().Be("user-b@tenant-b.test");
    }

    /// <summary>Cross-tenant mutation is forbidden when tenant header is scoped.</summary>
    [Fact]
    public async Task CreateUser_CrossTenant_ReturnsForbidden()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Tenant-A");
        var tenantB = await ProvisionTenantAsync(client, "Tenant-B");

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/users")
        {
            Content = JsonContent.Create(new { tenantId = tenantB, email = "cross@tenant-b.test" })
        };
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Tenant-Id", tenantA.ToString());
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    /// <summary>Invalid tenant header is rejected at middleware when RequireTenantHeader is enabled.</summary>
    [Fact]
    public async Task InvalidTenantHeader_WhenRequired_ReturnsBadRequest()
    {
        await using var factory = new TenantHeaderRequiredWebApplicationFactory();
        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/identity/users");
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Tenant-Id", "not-a-guid");
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static async Task<Guid> ProvisionTenantAsync(HttpClient client, string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/tenants")
        {
            Content = JsonContent.Create(new { name })
        };
        request.Headers.Add("X-Api-Key", _apiKey);
        var response = await client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var document = await response.Content.ReadFromJsonAsync<JsonElement>();
        return document.GetProperty("id").GetGuid();
    }

    private static async Task CreateUserAsync(HttpClient client, Guid tenantId, string email)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/users")
        {
            Content = JsonContent.Create(new { tenantId, email })
        };
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var response = await client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task<JsonElement[]> ListUsersAsync(HttpClient client, Guid tenantId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/identity/users");
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var response = await client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.Content.ReadFromJsonAsync<JsonElement[]>() ?? [];
    }

    private sealed class TenantHeaderRequiredWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _identityDbPath = Path.Combine(Path.GetTempPath(), $"aipm-identity-{Guid.NewGuid():N}.db");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Testing");
            builder.UseSetting("ConnectionStrings:PostgreSql", "");
            builder.UseSetting("ConnectionStrings:Redis", "inmemory");
            builder.UseSetting("ConnectionStrings:RabbitMq", "inmemory");
            builder.UseSetting("ConnectionStrings:IdentityDb", $"Data Source={_identityDbPath}");
            builder.UseSetting("Security:ApiKey", _apiKey);
            builder.UseSetting("Platform:RequireTenantHeader", "true");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && File.Exists(_identityDbPath))
            {
                try
                {
                    File.Delete(_identityDbPath);
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
