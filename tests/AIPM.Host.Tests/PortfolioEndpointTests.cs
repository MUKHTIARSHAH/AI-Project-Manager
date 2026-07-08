using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AIPM.Host.Tests;

public sealed class PortfolioEndpointTests
{
    private const string _apiKey = "test-bc10-key";

    [Fact]
    public async Task PortfolioEndpoints_RequireTenantHeader()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/portfolio");
        request.Headers.Add("X-Api-Key", _apiKey);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Can_Create_And_List_Portfolios_ForTenant()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();
        var tenantCreate = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/tenants")
        {
            Content = JsonContent.Create(new { name = "PortfolioTenant" })
        };
        tenantCreate.Headers.Add("X-Api-Key", _apiKey);
        var tenantCreateResponse = await client.SendAsync(tenantCreate);
        tenantCreateResponse.EnsureSuccessStatusCode();
        var tenantJson = await tenantCreateResponse.Content.ReadFromJsonAsync<TenantResponse>();
        tenantJson.Should().NotBeNull();
        var tenantId = tenantJson!.id.ToString();

        using var create = new HttpRequestMessage(HttpMethod.Post, "/api/v1/portfolio")
        {
            Content = JsonContent.Create(new { name = "Core Portfolio" })
        };
        create.Headers.Add("X-Api-Key", _apiKey);
        create.Headers.Add("X-Tenant-Id", tenantId);
        var createResponse = await client.SendAsync(create);
        createResponse.EnsureSuccessStatusCode();

        using var list = new HttpRequestMessage(HttpMethod.Get, "/api/v1/portfolio");
        list.Headers.Add("X-Api-Key", _apiKey);
        list.Headers.Add("X-Tenant-Id", tenantId);
        var listResponse = await client.SendAsync(list);
        listResponse.EnsureSuccessStatusCode();

        var payload = await listResponse.Content.ReadAsStringAsync();
        payload.Should().Contain("Core Portfolio");
    }

    private sealed record TenantResponse(Guid id, string name, string status);
}
