using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AIPM.Host.Tests;

public sealed class ProjectCloneEndpointTests
{
    private const string _apiKey = "test-bc10-key";

    [Fact]
    public async Task Clone_HappyPath_PreservesProgramOwnerWorkspace_StartsDraft()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "CloneTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@clone.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "PF", "PRG");
        var workspaceId = Guid.NewGuid();
        var sourceId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Source", workspaceId);

        using var clone = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{sourceId}/clone")
        {
            Content = JsonContent.Create(new { name = "Cloned Project" })
        };
        AddAuth(clone, tenantId);
        var cloneResponse = await client.SendAsync(clone);
        cloneResponse.EnsureSuccessStatusCode();
        var cloned = await cloneResponse.Content.ReadFromJsonAsync<CloneResponse>();

        cloned.Should().NotBeNull();
        cloned!.sourceProjectId.Should().Be(sourceId);
        cloned.id.Should().NotBe(sourceId);
        cloned.programId.Should().Be(programId);
        cloned.ownerUserId.Should().Be(ownerId);
        cloned.workspaceId.Should().Be(workspaceId);
        cloned.name.Should().Be("Cloned Project");
        cloned.status.Should().Be("Draft");
        cloned.archivedAt.Should().BeNull();

        using var getSource = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{sourceId}");
        AddAuth(getSource, tenantId);
        var source = await (await client.SendAsync(getSource)).Content.ReadFromJsonAsync<ProjectResponse>();
        source!.name.Should().Be("Source");
    }

    [Fact]
    public async Task Clone_CrossTenant_ReturnsNotFound()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Clone-A");
        var tenantB = await ProvisionTenantAsync(client, "Clone-B");
        var ownerA = await CreateUserAsync(client, tenantA, "a@clone.test");
        var programA = await CreateProgramHierarchyAsync(client, tenantA, "PA", "ProgA");
        var sourceId = await CreateProjectAsync(client, tenantA, programA, ownerA, "A-Source");

        using var clone = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{sourceId}/clone")
        {
            Content = JsonContent.Create(new { name = "Stolen" })
        };
        AddAuth(clone, tenantB);
        var response = await client.SendAsync(clone);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Clone_Archived_ReturnsBadRequest()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "CloneArchived");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@clone-arch.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "PF", "PRG");
        var sourceId = await CreateProjectAsync(client, tenantId, programId, ownerId, "To Archive");

        using var archive = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{sourceId}/archive");
        AddAuth(archive, tenantId);
        (await client.SendAsync(archive)).EnsureSuccessStatusCode();

        using var clone = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{sourceId}/clone")
        {
            Content = JsonContent.Create(new { name = "Clone Archived" })
        };
        AddAuth(clone, tenantId);
        var response = await client.SendAsync(clone);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Clone_DoesNotCopyScopeChanges()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "CloneScope");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@clone-scope.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "PF", "PRG");
        var sourceId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Scoped Source");

        using var record = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{sourceId}/scope-changes")
        {
            Content = JsonContent.Create(new
            {
                title = "Change",
                description = "Body",
                affectedRequirementCitation = "REQ-1"
            })
        };
        AddAuth(record, tenantId);
        (await client.SendAsync(record)).EnsureSuccessStatusCode();

        using var clone = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{sourceId}/clone")
        {
            Content = JsonContent.Create(new { name = "Clean Clone" })
        };
        AddAuth(clone, tenantId);
        var cloneResponse = await client.SendAsync(clone);
        cloneResponse.EnsureSuccessStatusCode();
        var cloned = await cloneResponse.Content.ReadFromJsonAsync<CloneResponse>();

        using var listClone = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{cloned!.id}/scope-changes");
        AddAuth(listClone, tenantId);
        var listClonePayload = await (await client.SendAsync(listClone)).Content.ReadAsStringAsync();
        listClonePayload.Should().Be("[]");

        using var listSource = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{sourceId}/scope-changes");
        AddAuth(listSource, tenantId);
        var listSourcePayload = await (await client.SendAsync(listSource)).Content.ReadAsStringAsync();
        listSourcePayload.Should().Contain("Change");
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
        var portfolioResponse = await client.SendAsync(createPortfolio);
        portfolioResponse.EnsureSuccessStatusCode();
        var portfolio = await portfolioResponse.Content.ReadFromJsonAsync<IdResponse>();

        using var createProgram = new HttpRequestMessage(HttpMethod.Post, "/api/v1/programs")
        {
            Content = JsonContent.Create(new { portfolioId = portfolio!.id, name = programName })
        };
        AddAuth(createProgram, tenantId);
        var programResponse = await client.SendAsync(createProgram);
        programResponse.EnsureSuccessStatusCode();
        var program = await programResponse.Content.ReadFromJsonAsync<IdResponse>();
        return program!.id;
    }

    private static async Task<Guid> CreateProjectAsync(
        HttpClient client,
        Guid tenantId,
        Guid programId,
        Guid ownerUserId,
        string name,
        Guid? workspaceId = null)
    {
        using var create = new HttpRequestMessage(HttpMethod.Post, "/api/v1/projects")
        {
            Content = JsonContent.Create(new
            {
                programId,
                workspaceId = workspaceId ?? Guid.NewGuid(),
                ownerUserId,
                name
            })
        };
        AddAuth(create, tenantId);
        var response = await client.SendAsync(create);
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        return project!.id;
    }

    private sealed record TenantResponse(Guid id, string name, string status);
    private sealed record UserResponse(Guid id, Guid tenantId, string email);
    private sealed record IdResponse(Guid id);
    private sealed record ProjectResponse(
        Guid id,
        Guid tenantId,
        Guid programId,
        Guid workspaceId,
        Guid ownerUserId,
        string name,
        string status,
        DateTimeOffset createdAt,
        DateTimeOffset? archivedAt);
    private sealed record CloneResponse(
        Guid sourceProjectId,
        Guid id,
        Guid tenantId,
        Guid programId,
        Guid workspaceId,
        Guid ownerUserId,
        string name,
        string status,
        DateTimeOffset createdAt,
        DateTimeOffset? archivedAt);
}
