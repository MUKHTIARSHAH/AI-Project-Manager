using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AIPM.Host.Tests;

public sealed class ProjectEndpointTests
{
    private const string _apiKey = "test-bc10-key";

    [Fact]
    public async Task ProjectEndpoints_RequireTenantHeader()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/projects");
        request.Headers.Add("X-Api-Key", _apiKey);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Can_Create_Get_Update_List_And_Archive_Project()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ProjectTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@project.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "Portfolio A", "Program A");
        var workspaceId = Guid.NewGuid();

        using var create = new HttpRequestMessage(HttpMethod.Post, "/api/v1/projects")
        {
            Content = JsonContent.Create(new
            {
                programId,
                workspaceId,
                ownerUserId = ownerId,
                name = "Mobile App v2"
            })
        };
        create.Headers.Add("X-Api-Key", _apiKey);
        create.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var createResponse = await client.SendAsync(create);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        created.Should().NotBeNull();
        created!.name.Should().Be("Mobile App v2");
        created.status.Should().Be("Draft");

        using var get = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{created.id}");
        get.Headers.Add("X-Api-Key", _apiKey);
        get.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var getResponse = await client.SendAsync(get);
        getResponse.EnsureSuccessStatusCode();

        var newWorkspace = Guid.NewGuid();
        using var update = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/projects/{created.id}")
        {
            Content = JsonContent.Create(new
            {
                workspaceId = newWorkspace,
                ownerUserId = ownerId,
                name = "Mobile App v2.1"
            })
        };
        update.Headers.Add("X-Api-Key", _apiKey);
        update.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var updateResponse = await client.SendAsync(update);
        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        updated!.name.Should().Be("Mobile App v2.1");
        updated.workspaceId.Should().Be(newWorkspace);

        using var list = new HttpRequestMessage(HttpMethod.Get, "/api/v1/projects");
        list.Headers.Add("X-Api-Key", _apiKey);
        list.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var listResponse = await client.SendAsync(list);
        listResponse.EnsureSuccessStatusCode();
        var listPayload = await listResponse.Content.ReadAsStringAsync();
        listPayload.Should().Contain("Mobile App v2.1");

        using var archive = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{created.id}/archive");
        archive.Headers.Add("X-Api-Key", _apiKey);
        archive.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var archiveResponse = await client.SendAsync(archive);
        archiveResponse.EnsureSuccessStatusCode();
        var archived = await archiveResponse.Content.ReadFromJsonAsync<ProjectResponse>();
        archived!.status.Should().Be("Archived");
        archived.archivedAt.Should().NotBeNull();

        using var updateArchived = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/projects/{created.id}")
        {
            Content = JsonContent.Create(new
            {
                workspaceId = newWorkspace,
                ownerUserId = ownerId,
                name = "Should Fail"
            })
        };
        updateArchived.Headers.Add("X-Api-Key", _apiKey);
        updateArchived.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var blocked = await client.SendAsync(updateArchived);
        blocked.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListProjects_FiltersByTenant_And_CrossTenantGetReturnsNotFound()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Project-Tenant-A");
        var tenantB = await ProvisionTenantAsync(client, "Project-Tenant-B");
        var ownerA = await CreateUserAsync(client, tenantA, "a@tenant-a.test");
        var ownerB = await CreateUserAsync(client, tenantB, "b@tenant-b.test");
        var programA = await CreateProgramHierarchyAsync(client, tenantA, "PA", "ProgA");
        var programB = await CreateProgramHierarchyAsync(client, tenantB, "PB", "ProgB");

        var projectA = await CreateProjectAsync(client, tenantA, programA, ownerA, "A-Only");
        await CreateProjectAsync(client, tenantB, programB, ownerB, "B-Only");

        using var listA = new HttpRequestMessage(HttpMethod.Get, "/api/v1/projects");
        listA.Headers.Add("X-Api-Key", _apiKey);
        listA.Headers.Add("X-Tenant-Id", tenantA.ToString());
        var listAResponse = await client.SendAsync(listA);
        listAResponse.EnsureSuccessStatusCode();
        var payloadA = await listAResponse.Content.ReadAsStringAsync();
        payloadA.Should().Contain("A-Only");
        payloadA.Should().NotContain("B-Only");

        using var crossGet = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{projectA}");
        crossGet.Headers.Add("X-Api-Key", _apiKey);
        crossGet.Headers.Add("X-Tenant-Id", tenantB.ToString());
        var crossResponse = await client.SendAsync(crossGet);
        crossResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProject_CrossTenantOwner_ReturnsForbidden()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Owner-Tenant-A");
        var tenantB = await ProvisionTenantAsync(client, "Owner-Tenant-B");
        var ownerB = await CreateUserAsync(client, tenantB, "owner-b@tenant-b.test");
        var programA = await CreateProgramHierarchyAsync(client, tenantA, "PA", "ProgA");

        using var create = new HttpRequestMessage(HttpMethod.Post, "/api/v1/projects")
        {
            Content = JsonContent.Create(new
            {
                programId = programA,
                workspaceId = Guid.NewGuid(),
                ownerUserId = ownerB,
                name = "Cross Owner"
            })
        };
        create.Headers.Add("X-Api-Key", _apiKey);
        create.Headers.Add("X-Tenant-Id", tenantA.ToString());
        var response = await client.SendAsync(create);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
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
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Tenant-Id", tenantId.ToString());
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
        createPortfolio.Headers.Add("X-Api-Key", _apiKey);
        createPortfolio.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var portfolioResponse = await client.SendAsync(createPortfolio);
        portfolioResponse.EnsureSuccessStatusCode();
        var portfolio = await portfolioResponse.Content.ReadFromJsonAsync<IdResponse>();

        using var createProgram = new HttpRequestMessage(HttpMethod.Post, "/api/v1/programs")
        {
            Content = JsonContent.Create(new { portfolioId = portfolio!.id, name = programName })
        };
        createProgram.Headers.Add("X-Api-Key", _apiKey);
        createProgram.Headers.Add("X-Tenant-Id", tenantId.ToString());
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
        create.Headers.Add("X-Api-Key", _apiKey);
        create.Headers.Add("X-Tenant-Id", tenantId.ToString());
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
}
