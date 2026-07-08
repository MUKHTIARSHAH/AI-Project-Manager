using System.Net.Http.Json;
using FluentAssertions;

namespace AIPM.Host.Tests;

public sealed class ProgramEndpointTests
{
    private const string _apiKey = "test-bc10-key";

    [Fact]
    public async Task Can_Create_And_List_Programs()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ProgramTenant");
        var portfolioId = await CreatePortfolioAsync(client, tenantId, "Portfolio A");

        using var createProgram = new HttpRequestMessage(HttpMethod.Post, "/api/v1/programs")
        {
            Content = JsonContent.Create(new { portfolioId, name = "Program A" })
        };
        createProgram.Headers.Add("X-Api-Key", _apiKey);
        createProgram.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var createResponse = await client.SendAsync(createProgram);
        createResponse.EnsureSuccessStatusCode();

        using var listPrograms = new HttpRequestMessage(HttpMethod.Get, "/api/v1/programs");
        listPrograms.Headers.Add("X-Api-Key", _apiKey);
        listPrograms.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var listResponse = await client.SendAsync(listPrograms);
        listResponse.EnsureSuccessStatusCode();
        var payload = await listResponse.Content.ReadAsStringAsync();
        payload.Should().Contain("Program A");
    }

    private static async Task<Guid> ProvisionTenantAsync(HttpClient client, string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/tenants")
        {
            Content = JsonContent.Create(new { name })
        };
        request.Headers.Add("X-Api-Key", _apiKey);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var tenant = await response.Content.ReadFromJsonAsync<TenantResponse>();
        return tenant!.id;
    }

    private static async Task<Guid> CreatePortfolioAsync(HttpClient client, Guid tenantId, string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/portfolio")
        {
            Content = JsonContent.Create(new { name })
        };
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var portfolio = await response.Content.ReadFromJsonAsync<PortfolioResponse>();
        return portfolio!.id;
    }

    private sealed record TenantResponse(Guid id, string name, string status);
    private sealed record PortfolioResponse(Guid id, Guid tenantId, string name, DateTimeOffset createdAt);
}
