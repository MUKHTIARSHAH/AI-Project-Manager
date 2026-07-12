using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AIPM.Host.Tests;

public sealed class ScopeChangeEndpointTests
{
    private const string _apiKey = "test-bc10-key";

    [Fact]
    public async Task ScopeChange_HappyPath_RecordListGetApproveImplement()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ScopeTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@scope.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "PF", "PRG");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Scope Project");

        using var record = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/scope-changes")
        {
            Content = JsonContent.Create(new
            {
                title = "Add OAuth2",
                description = "Extend login scope",
                affectedRequirementCitation = "REQ-1"
            })
        };
        AddAuth(record, tenantId);
        var recordResponse = await client.SendAsync(record);
        recordResponse.EnsureSuccessStatusCode();
        var created = await recordResponse.Content.ReadFromJsonAsync<ScopeChangeResponse>();
        created.Should().NotBeNull();
        created!.status.Should().Be("Proposed");
        created.title.Should().Be("Add OAuth2");

        using var list = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{projectId}/scope-changes");
        AddAuth(list, tenantId);
        var listResponse = await client.SendAsync(list);
        listResponse.EnsureSuccessStatusCode();
        var listPayload = await listResponse.Content.ReadAsStringAsync();
        listPayload.Should().Contain("Add OAuth2");

        using var get = new HttpRequestMessage(
            HttpMethod.Get,
            $"/api/v1/projects/{projectId}/scope-changes/{created.id}");
        AddAuth(get, tenantId);
        (await client.SendAsync(get)).EnsureSuccessStatusCode();

        using var approve = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/projects/{projectId}/scope-changes/{created.id}/approve");
        AddAuth(approve, tenantId);
        var approveResponse = await client.SendAsync(approve);
        approveResponse.EnsureSuccessStatusCode();
        var approved = await approveResponse.Content.ReadFromJsonAsync<ScopeChangeResponse>();
        approved!.status.Should().Be("Approved");

        using var implement = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/projects/{projectId}/scope-changes/{created.id}/implement");
        AddAuth(implement, tenantId);
        var implementResponse = await client.SendAsync(implement);
        implementResponse.EnsureSuccessStatusCode();
        var implemented = await implementResponse.Content.ReadFromJsonAsync<ScopeChangeResponse>();
        implemented!.status.Should().Be("Implemented");
    }

    [Fact]
    public async Task ScopeChange_CrossTenantProject_ReturnsNotFound()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Scope-A");
        var tenantB = await ProvisionTenantAsync(client, "Scope-B");
        var ownerA = await CreateUserAsync(client, tenantA, "a@scope.test");
        var programA = await CreateProgramHierarchyAsync(client, tenantA, "PA", "ProgA");
        var projectA = await CreateProjectAsync(client, tenantA, programA, ownerA, "A Project");

        using var record = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectA}/scope-changes")
        {
            Content = JsonContent.Create(new { title = "X", description = "Y", affectedRequirementCitation = (string?)null })
        };
        AddAuth(record, tenantB);
        var response = await client.SendAsync(record);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ScopeChange_OnArchivedProject_ReturnsBadRequest()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ScopeArchived");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@archived-scope.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "PF", "PRG");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Archived Host");

        using var archive = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/archive");
        AddAuth(archive, tenantId);
        (await client.SendAsync(archive)).EnsureSuccessStatusCode();

        using var record = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/scope-changes")
        {
            Content = JsonContent.Create(new { title = "Nope", description = "Blocked", affectedRequirementCitation = (string?)null })
        };
        AddAuth(record, tenantId);
        var response = await client.SendAsync(record);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ScopeChange_RejectPath_Works()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ScopeReject");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@reject.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "PF", "PRG");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Reject Host");

        using var record = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/scope-changes")
        {
            Content = JsonContent.Create(new { title = "Reject Me", description = "Not needed", affectedRequirementCitation = (string?)null })
        };
        AddAuth(record, tenantId);
        var created = await (await client.SendAsync(record)).Content.ReadFromJsonAsync<ScopeChangeResponse>();

        using var reject = new HttpRequestMessage(
            HttpMethod.Post,
            $"/api/v1/projects/{projectId}/scope-changes/{created!.id}/reject");
        AddAuth(reject, tenantId);
        var rejectResponse = await client.SendAsync(reject);
        rejectResponse.EnsureSuccessStatusCode();
        var rejected = await rejectResponse.Content.ReadFromJsonAsync<ScopeChangeResponse>();
        rejected!.status.Should().Be("Rejected");
    }

    private static void AddAuth(HttpRequestMessage request, Guid tenantId)
    {
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Tenant-Id", tenantId.ToString());
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

    private static async Task<Guid> CreateUserAsync(HttpClient client, Guid tenantId, string email)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/users")
        {
            Content = JsonContent.Create(new { tenantId, email })
        };
        AddAuth(request, tenantId);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        return user!.id;
    }

    private static async Task<Guid> CreateProgramHierarchyAsync(
        HttpClient client,
        Guid tenantId,
        string portfolioName,
        string programName)
    {
        using var createPortfolio = new HttpRequestMessage(HttpMethod.Post, "/api/v1/portfolio")
        {
            Content = JsonContent.Create(new { name = portfolioName })
        };
        AddAuth(createPortfolio, tenantId);
        var portfolio = await (await client.SendAsync(createPortfolio)).Content.ReadFromJsonAsync<IdResponse>();

        using var createProgram = new HttpRequestMessage(HttpMethod.Post, "/api/v1/programs")
        {
            Content = JsonContent.Create(new { portfolioId = portfolio!.id, name = programName })
        };
        AddAuth(createProgram, tenantId);
        var program = await (await client.SendAsync(createProgram)).Content.ReadFromJsonAsync<IdResponse>();
        return program!.id;
    }

    private static async Task<Guid> CreateProjectAsync(
        HttpClient client,
        Guid tenantId,
        Guid programId,
        Guid ownerUserId,
        string name)
    {
        using var create = new HttpRequestMessage(HttpMethod.Post, "/api/v1/projects")
        {
            Content = JsonContent.Create(new
            {
                programId,
                workspaceId = Guid.NewGuid(),
                ownerUserId,
                name
            })
        };
        AddAuth(create, tenantId);
        var project = await (await client.SendAsync(create)).Content.ReadFromJsonAsync<IdResponse>();
        return project!.id;
    }

    private sealed record TenantResponse(Guid id, string name, string status);
    private sealed record UserResponse(Guid id, Guid tenantId, string email);
    private sealed record IdResponse(Guid id);
    private sealed record ScopeChangeResponse(
        Guid id,
        Guid projectId,
        Guid tenantId,
        string title,
        string description,
        string? affectedRequirementCitation,
        string status,
        DateTimeOffset recordedAt,
        DateTimeOffset? decidedAt);
}
