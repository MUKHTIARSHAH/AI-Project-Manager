using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AIPM.Host.Tests;

public sealed class ProjectionSummaryEndpointTests
{
    private const string _apiKey = "test-bc10-key";

    [Fact]
    public async Task Summaries_HappyPath_ReconcileToChildCounts()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ProjTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@proj.test");
        var (portfolioId, programId) = await CreateHierarchyAsync(client, tenantId, "PF", "PRG");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Alpha");
        await CreateProjectAsync(client, tenantId, programId, ownerId, "Beta");

        using var record = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/scope-changes")
        {
            Content = JsonContent.Create(new { title = "T", description = "D", affectedRequirementCitation = (string?)null })
        };
        AddAuth(record, tenantId);
        (await client.SendAsync(record)).EnsureSuccessStatusCode();

        using var portfolioSummary = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/portfolio/{portfolioId}/summary");
        AddAuth(portfolioSummary, tenantId);
        var portfolio = await (await client.SendAsync(portfolioSummary)).Content.ReadFromJsonAsync<PortfolioSummaryResponse>();
        portfolio.Should().NotBeNull();
        portfolio!.programCount.Should().Be(1);
        portfolio.projectCount.Should().Be(2);
        portfolio.draftProjectCount.Should().Be(2);

        using var programSummary = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/programs/{programId}/summary");
        AddAuth(programSummary, tenantId);
        var program = await (await client.SendAsync(programSummary)).Content.ReadFromJsonAsync<ProgramSummaryResponse>();
        program!.projectCount.Should().Be(2);
        program.portfolioId.Should().Be(portfolioId);

        using var projectSummary = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{projectId}/summary");
        AddAuth(projectSummary, tenantId);
        var project = await (await client.SendAsync(projectSummary)).Content.ReadFromJsonAsync<ProjectSummaryResponse>();
        project!.scopeChangeCount.Should().Be(1);
        project.portfolioId.Should().Be(portfolioId);
        project.programId.Should().Be(programId);
        project.status.Should().Be("Draft");
    }

    [Fact]
    public async Task Summaries_CrossTenant_ReturnNotFound()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Sum-A");
        var tenantB = await ProvisionTenantAsync(client, "Sum-B");
        var ownerA = await CreateUserAsync(client, tenantA, "a@sum.test");
        var (portfolioA, programA) = await CreateHierarchyAsync(client, tenantA, "PA", "ProgA");
        var projectA = await CreateProjectAsync(client, tenantA, programA, ownerA, "OnlyA");

        using var portfolioReq = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/portfolio/{portfolioA}/summary");
        AddAuth(portfolioReq, tenantB);
        (await client.SendAsync(portfolioReq)).StatusCode.Should().Be(HttpStatusCode.NotFound);

        using var programReq = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/programs/{programA}/summary");
        AddAuth(programReq, tenantB);
        (await client.SendAsync(programReq)).StatusCode.Should().Be(HttpStatusCode.NotFound);

        using var projectReq = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{projectA}/summary");
        AddAuth(projectReq, tenantB);
        (await client.SendAsync(projectReq)).StatusCode.Should().Be(HttpStatusCode.NotFound);
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

    private static async Task<(Guid PortfolioId, Guid ProgramId)> CreateHierarchyAsync(
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
        return (portfolio.id, program!.id);
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
    private sealed record PortfolioSummaryResponse(
        Guid portfolioId,
        string portfolioName,
        int programCount,
        int projectCount,
        int draftProjectCount,
        int activeProjectCount,
        int onHoldProjectCount,
        int completedProjectCount,
        int archivedProjectCount);
    private sealed record ProgramSummaryResponse(
        Guid programId,
        string programName,
        Guid portfolioId,
        int projectCount,
        int draftProjectCount,
        int activeProjectCount,
        int onHoldProjectCount,
        int completedProjectCount,
        int archivedProjectCount);
    private sealed record ProjectSummaryResponse(
        Guid projectId,
        string projectName,
        Guid programId,
        Guid portfolioId,
        Guid ownerUserId,
        Guid workspaceId,
        string status,
        int scopeChangeCount);
}
