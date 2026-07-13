using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;

namespace AIPM.Host.Tests;

public sealed class RequirementEndpointTests
{
    private const string _apiKey = "test-bc10-key";

    [Fact]
    public async Task RequirementEndpoints_RequireTenantHeader()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{Guid.NewGuid()}/requirements");
        request.Headers.Add("X-Api-Key", _apiKey);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Can_Intake_List_And_Get_Requirement_AsDraftWithParsed()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ReqTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner@req.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "Portfolio R", "Program R");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Delivery");

        using var intake = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/requirements")
        {
            Content = JsonContent.Create(new
            {
                title = "User login",
                statement = "Users authenticate via email and password.",
                acceptanceCriteria = new[] { "Invalid password shows error", "Valid login lands on home" },
                documentTitle = "login-spec.md",
                documentContentType = "text/markdown",
                documentUriOrName = "docs/login-spec.md"
            })
        };
        intake.Headers.Add("X-Api-Key", _apiKey);
        intake.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var intakeResponse = await client.SendAsync(intake);
        intakeResponse.EnsureSuccessStatusCode();
        var created = await intakeResponse.Content.ReadFromJsonAsync<RequirementResponse>();
        created.Should().NotBeNull();
        created!.title.Should().Be("User login");
        created.status.Should().Be("Draft");
        created.parsed.Should().BeTrue();
        created.acceptanceCriteria.Should().HaveCount(2);
        created.documentTitle.Should().Be("login-spec.md");

        using var list = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{projectId}/requirements");
        list.Headers.Add("X-Api-Key", _apiKey);
        list.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var listResponse = await client.SendAsync(list);
        listResponse.EnsureSuccessStatusCode();
        var listPayload = await listResponse.Content.ReadAsStringAsync();
        listPayload.Should().Contain("User login");
        listPayload.Should().Contain("Draft");

        using var get = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/requirements/{created.id}");
        get.Headers.Add("X-Api-Key", _apiKey);
        get.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var getResponse = await client.SendAsync(get);
        getResponse.EnsureSuccessStatusCode();
        var got = await getResponse.Content.ReadFromJsonAsync<RequirementResponse>();
        got!.parsed.Should().BeTrue();
        got.status.Should().Be("Draft");
    }

    [Fact]
    public async Task ListAndGet_FilterByTenant_And_ArchivedProjectBlocksIntake()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantA = await ProvisionTenantAsync(client, "Req-Tenant-A");
        var tenantB = await ProvisionTenantAsync(client, "Req-Tenant-B");
        var ownerA = await CreateUserAsync(client, tenantA, "a@req-a.test");
        var ownerB = await CreateUserAsync(client, tenantB, "b@req-b.test");
        var programA = await CreateProgramHierarchyAsync(client, tenantA, "PA", "ProgA");
        var programB = await CreateProgramHierarchyAsync(client, tenantB, "PB", "ProgB");
        var projectA = await CreateProjectAsync(client, tenantA, programA, ownerA, "A-Project");
        var projectB = await CreateProjectAsync(client, tenantB, programB, ownerB, "B-Project");

        var requirementA = await IntakeRequirementAsync(client, tenantA, projectA, "A-Only");
        await IntakeRequirementAsync(client, tenantB, projectB, "B-Only");

        using var listA = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/projects/{projectA}/requirements");
        listA.Headers.Add("X-Api-Key", _apiKey);
        listA.Headers.Add("X-Tenant-Id", tenantA.ToString());
        var listAResponse = await client.SendAsync(listA);
        listAResponse.EnsureSuccessStatusCode();
        var payloadA = await listAResponse.Content.ReadAsStringAsync();
        payloadA.Should().Contain("A-Only");
        payloadA.Should().NotContain("B-Only");
        using var crossGet = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/requirements/{requirementA}");
        crossGet.Headers.Add("X-Api-Key", _apiKey);
        crossGet.Headers.Add("X-Tenant-Id", tenantB.ToString());
        var crossResponse = await client.SendAsync(crossGet);
        crossResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        using var archive = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectA}/archive");
        archive.Headers.Add("X-Api-Key", _apiKey);
        archive.Headers.Add("X-Tenant-Id", tenantA.ToString());
        (await client.SendAsync(archive)).EnsureSuccessStatusCode();

        using var blockedIntake = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectA}/requirements")
        {
            Content = JsonContent.Create(new { title = "Nope", statement = "Should fail" })
        };
        blockedIntake.Headers.Add("X-Api-Key", _apiKey);
        blockedIntake.Headers.Add("X-Tenant-Id", tenantA.ToString());
        var blocked = await client.SendAsync(blockedIntake);
        blocked.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IntakeRequirement_ReturnsProblemDetails_ForMissingTitle()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ReqInvalidTitleTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner-invalid-title@req.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "Portfolio Invalid", "Program Invalid");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Invalid Title Project");

        using var intake = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/requirements")
        {
            Content = JsonContent.Create(new { statement = "Missing title statement" })
        };
        intake.Headers.Add("X-Api-Key", _apiKey);
        intake.Headers.Add("X-Tenant-Id", tenantId.ToString());

        var response = await client.SendAsync(intake);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await ReadProblemDetailsAsync(response);
        problem.GetProperty("type").GetString().Should().Contain("validation-error");
        problem.GetProperty("detail").GetString().Should().Contain("title");
    }

    [Fact]
    public async Task IntakeRequirement_ReturnsProblemDetails_ForMissingStatement()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ReqInvalidStatementTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner-invalid-statement@req.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "Portfolio Invalid", "Program Invalid");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Invalid Statement Project");

        using var intake = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/requirements")
        {
            Content = JsonContent.Create(new { title = "Missing statement" })
        };
        intake.Headers.Add("X-Api-Key", _apiKey);
        intake.Headers.Add("X-Tenant-Id", tenantId.ToString());

        var response = await client.SendAsync(intake);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await ReadProblemDetailsAsync(response);
        problem.GetProperty("type").GetString().Should().Contain("validation-error");
        problem.GetProperty("detail").GetString().Should().Contain("statement");
    }

    [Fact]
    public async Task IntakeRequirement_ReturnsProblemDetails_ForEmptyTitle()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ReqEmptyTitleTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner-empty-title@req.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "Portfolio Empty Title", "Program Empty Title");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Empty Title Project");

        using var intake = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/requirements")
        {
            Content = JsonContent.Create(new { title = " ", statement = "Statement exists" })
        };
        intake.Headers.Add("X-Api-Key", _apiKey);
        intake.Headers.Add("X-Tenant-Id", tenantId.ToString());

        var response = await client.SendAsync(intake);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await ReadProblemDetailsAsync(response);
        problem.GetProperty("type").GetString().Should().Contain("validation-error");
        problem.GetProperty("detail").GetString().Should().Contain("title");
    }

    [Fact]
    public async Task IntakeRequirement_ReturnsProblemDetails_ForInvalidDocumentMetadata()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ReqInvalidDocumentMetadataTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner-invalid-doc@req.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "Portfolio Invalid Doc", "Program Invalid Doc");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Invalid Doc Project");

        using var intake = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/requirements")
        {
            Content = JsonContent.Create(new
            {
                title = "Bad document metadata",
                statement = "Statement is valid.",
                documentTitle = new string('x', 201)
            })
        };
        intake.Headers.Add("X-Api-Key", _apiKey);
        intake.Headers.Add("X-Tenant-Id", tenantId.ToString());

        var response = await client.SendAsync(intake);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await ReadProblemDetailsAsync(response);
        problem.GetProperty("type").GetString().Should().Contain("validation-error");
        problem.GetProperty("detail").GetString().Should().Contain("Document title must be at most 200 characters");
    }

    [Fact]
    public async Task IntakeRequirement_ReturnsProblemDetails_ForMalformedRequestBody()
    {
        await using var factory = new HostTestWebApplicationFactory();
        using var client = factory.CreateClient();

        var tenantId = await ProvisionTenantAsync(client, "ReqMalformedBodyTenant");
        var ownerId = await CreateUserAsync(client, tenantId, "owner-malformed@req.test");
        var programId = await CreateProgramHierarchyAsync(client, tenantId, "Portfolio Malformed", "Program Malformed");
        var projectId = await CreateProjectAsync(client, tenantId, programId, ownerId, "Malformed Project");

        using var intake = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/requirements")
        {
            Content = new StringContent("{\"title\": \"Broken JSON\"", Encoding.UTF8, "application/json")
        };
        intake.Headers.Add("X-Api-Key", _apiKey);
        intake.Headers.Add("X-Tenant-Id", tenantId.ToString());

        var response = await client.SendAsync(intake);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await ReadProblemDetailsAsync(response);
        problem.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
        problem.GetProperty("type").GetString().Should().NotBeNullOrWhiteSpace();
    }

    private static async Task<JsonElement> ReadProblemDetailsAsync(HttpResponseMessage response)
    {
        response.Content.Headers.ContentType?.MediaType.Should().StartWith("application/problem+json");
        var payload = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(payload);
        var root = document.RootElement;
        root.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
        return root;
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

    private static async Task<Guid> IntakeRequirementAsync(
        HttpClient client,
        Guid tenantId,
        Guid projectId,
        string title)
    {
        using var intake = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/projects/{projectId}/requirements")
        {
            Content = JsonContent.Create(new { title, statement = $"{title} statement" })
        };
        intake.Headers.Add("X-Api-Key", _apiKey);
        intake.Headers.Add("X-Tenant-Id", tenantId.ToString());
        var response = await client.SendAsync(intake);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<RequirementResponse>();
        return created!.id;
    }

    private sealed record TenantResponse(Guid id, string name, string status);
    private sealed record UserResponse(Guid id, Guid tenantId, string email);
    private sealed record IdResponse(Guid id);
    private sealed record ProjectResponse(Guid id, string name, string status);
    private sealed record AcceptanceCriterionResponse(Guid id, string statement, int sortOrder);
    private sealed record RequirementResponse(
        Guid id,
        Guid tenantId,
        Guid projectId,
        string title,
        string statement,
        string status,
        bool parsed,
        string? documentTitle,
        string? documentContentType,
        string? documentUriOrName,
        DateTimeOffset createdAt,
        IReadOnlyList<AcceptanceCriterionResponse> acceptanceCriteria);
}
