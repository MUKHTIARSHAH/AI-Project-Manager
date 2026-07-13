using AIPM.Application;
using AIPM.Application.AI;
using AIPM.Application.Identity.Commands;
using AIPM.Application.Identity.Queries;
using AIPM.Application.Platform;
using AIPM.Application.Portfolio.Commands;
using AIPM.Application.Portfolio.Queries;
using AIPM.Application.Requirements.Commands;
using AIPM.Application.Requirements.Queries;
using AIPM.Application.Runtime;
using AIPM.Application.Runtime.Agents;
using AIPM.Application.Runtime.Contracts;
using AIPM.Application.Runtime.Plugins;
using AIPM.Host.Http;
using AIPM.Host.Runtime;
using AIPM.Host.Security;
using AIPM.Infrastructure;
using AIPM.Infrastructure.Configuration;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Messaging;
using AIPM.Infrastructure.Messaging.Health;
using AIPM.Plugins;
using AIPM.Workflow;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;



var builder = WebApplication.CreateBuilder(args);



builder.Configuration.AddDeploymentProfile(builder.Environment, builder.Environment.ContentRootPath);



Log.Logger = new LoggerConfiguration()

    .ReadFrom.Configuration(builder.Configuration)

    .Enrich.FromLogContext()

    .Enrich.WithEnvironmentName()

    .Enrich.WithThreadId()

    .WriteTo.Console()

    .CreateLogger();



builder.Host.UseSerilog();



builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();



builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services
    .AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bc10Admin", policy =>
        policy.RequireAuthenticatedUser().RequireRole("PlatformAdmin"));
});

builder.Services.AddWorkflow();



var pluginsPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "plugins"));

var platformOptions = builder.Configuration.GetSection(PlatformOptions.SectionName).Get<PlatformOptions>()
    ?? new PlatformOptions();

builder.Services.AddPlugins(options =>
{
    options.ScanPaths = [pluginsPath];
    options.AllowUnsignedPlugins = platformOptions.AllowUnsignedPluginsDev;
});



builder.Services.AddHostedService<PlatformRuntimeHostedService>();



var otelOptions = builder.Configuration.GetSection(OpenTelemetryOptions.SectionName).Get<OpenTelemetryOptions>()

    ?? new OpenTelemetryOptions();

builder.Services.AddOpenTelemetry()

    .ConfigureResource(resource => resource.AddService(otelOptions.ServiceName))

    .WithTracing(tracing =>

    {

        tracing.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();

        if (!string.IsNullOrWhiteSpace(otelOptions.OtlpEndpoint))

        {

            tracing.AddOtlpExporter();

        }

    })

    .WithMetrics(metrics =>

    {

        metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();

        if (!string.IsNullOrWhiteSpace(otelOptions.OtlpEndpoint))

        {

            metrics.AddOtlpExporter();

        }

    });



var healthBuilder = builder.Services.AddHealthChecks()

    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("host alive"));

healthBuilder.AddCheck<MessagingPipelineHealthCheck>("messaging-pipeline");



static bool IsRealConnection(string? value) =>

    !string.IsNullOrWhiteSpace(value)

    && !value.Equals("inmemory", StringComparison.OrdinalIgnoreCase);



var postgresConnection = builder.Configuration.GetConnectionString("PostgreSql");

if (IsRealConnection(postgresConnection))

{

    healthBuilder.AddNpgSql(postgresConnection!, name: "postgresql");

}



var redisConnection = builder.Configuration.GetConnectionString("Redis");

if (IsRealConnection(redisConnection))

{

    healthBuilder.AddRedis(redisConnection!, name: "redis");

}



var rabbitConnection = builder.Configuration.GetConnectionString("RabbitMq");

if (IsRealConnection(rabbitConnection))

{

    healthBuilder.AddRabbitMQ(rabbitConnection!, name: "rabbitmq");

}



var app = builder.Build();



app.UseExceptionHandler();
app.UseMiddleware<TenantHeaderMiddleware>();
app.UseAuthentication();
app.UseAuthorization();



app.MapGet("/health", () => Results.Text("healthy")).AllowAnonymous();



app.MapHealthChecks("/ready").AllowAnonymous();



var apiV1 = app.MapGroup("/api/v1");



apiV1.MapGet("/platform/ping", async (IMediator mediator, CancellationToken ct) =>

{

    var response = await mediator.Send(new PlatformPingCommand(), ct);

    return Results.Ok(response);

});



apiV1.MapGet("/platform/deployment", (IOptions<DeploymentOptions> deployment) =>

    Results.Ok(new

    {

        profile = deployment.Value.Profile,

        capabilities = deployment.Value.Capabilities

    }));

apiV1.MapGet("/ai/providers", (IAiProviderRegistry registry) =>
    Results.Ok(new
    {
        providers = registry.ListProviders(),
        mode = "abstraction-only"
    }));



apiV1.MapGet("/agents", (IAgentRegistry registry) => Results.Ok(registry.List()));

apiV1.MapGet("/agent-types", (IAgentRegistry registry) =>
{
    var contracts = registry.List()
        .Select(AgentTypeContractMapper.ToContract)
        .ToList();

    return Results.Ok(new AgentTypeCatalogResponse(AgentSdkContract.SchemaVersion, contracts));
});



apiV1.MapGet("/agents/{capability}", (string capability, IAgentRegistry registry) =>

{

    var descriptor = registry.ResolveByCapability(capability)

        ?? throw new AIPM.SharedKernel.Errors.NotFoundError($"No agent registered for capability '{capability}'.");

    return Results.Ok(descriptor);

});



var demoEndpoint = apiV1.MapPost("/runtime/demo/echo", async (PlatformRuntimeOrchestrator orchestrator, CancellationToken ct) =>

{

    var result = await orchestrator.RunEchoAgentDemoAsync(ct);

    return Results.Ok(result);

});

if (!app.Environment.IsDevelopment())

{

    demoEndpoint.RequireAuthorization("Bc10Admin");

}



var identity = apiV1.MapGroup("/identity").RequireAuthorization("Bc10Admin");
identity.MapGet("/tenants", async (IMediator mediator, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ListTenantsQuery(), ct)));
identity.MapPost("/tenants", async (IMediator mediator, ProvisionTenantCommand command, CancellationToken ct) =>
    Results.Ok(await mediator.Send(command, ct)));
identity.MapPost("/tenants/{tenantId:guid}/suspend", async (IMediator mediator, Guid tenantId, CancellationToken ct) =>
{
    await mediator.Send(new SuspendTenantCommand(tenantId), ct);
    return Results.NoContent();
});
identity.MapGet("/users", async (IMediator mediator, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ListUsersQuery(), ct)));
identity.MapPost("/users", async (IMediator mediator, CreateUserCommand command, CancellationToken ct) =>
    Results.Ok(await mediator.Send(command, ct)));
identity.MapGet("/roles", async (IMediator mediator, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ListRolesQuery(), ct)));
identity.MapPost("/roles", async (IMediator mediator, CreateRoleCommand command, CancellationToken ct) =>
    Results.Ok(await mediator.Send(command, ct)));
identity.MapPost("/users/{userId:guid}/roles/{roleId:guid}", async (IMediator mediator, Guid userId, Guid roleId, CancellationToken ct) =>
{
    await mediator.Send(new AssignRoleCommand(userId, roleId), ct);
    return Results.NoContent();
});
identity.MapPost("/roles/{roleId:guid}/permissions", async (IMediator mediator, Guid roleId, AssignPermissionRequest request, CancellationToken ct) =>
{
    await mediator.Send(new AssignPermissionCommand(roleId, request.PermissionCode), ct);
    return Results.NoContent();
});

var portfolio = apiV1.MapGroup("/portfolio").RequireAuthorization("Bc10Admin");
portfolio.MapGet(string.Empty, async (IMediator mediator, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ListPortfoliosQuery(), ct)));
portfolio.MapGet("/{portfolioId:guid}", async (IMediator mediator, Guid portfolioId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetPortfolioQuery(portfolioId), ct)));
portfolio.MapGet("/{portfolioId:guid}/summary", async (IMediator mediator, Guid portfolioId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetPortfolioSummaryQuery(portfolioId), ct)));
portfolio.MapPost(string.Empty, async (IMediator mediator, CreatePortfolioCommand command, CancellationToken ct) =>
    Results.Ok(await mediator.Send(command, ct)));

var programs = apiV1.MapGroup("/programs").RequireAuthorization("Bc10Admin");
programs.MapGet(string.Empty, async (IMediator mediator, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ListProgramsQuery(), ct)));
programs.MapGet("/{programId:guid}", async (IMediator mediator, Guid programId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetProgramQuery(programId), ct)));
programs.MapGet("/{programId:guid}/summary", async (IMediator mediator, Guid programId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetProgramSummaryQuery(programId), ct)));
programs.MapPost(string.Empty, async (IMediator mediator, CreateProgramCommand command, CancellationToken ct) =>
    Results.Ok(await mediator.Send(command, ct)));

var projects = apiV1.MapGroup("/projects").RequireAuthorization("Bc10Admin");
projects.MapGet(string.Empty, async (IMediator mediator, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ListProjectsQuery(), ct)));
projects.MapGet("/{projectId:guid}", async (IMediator mediator, Guid projectId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetProjectQuery(projectId), ct)));
projects.MapGet("/{projectId:guid}/summary", async (IMediator mediator, Guid projectId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetProjectSummaryQuery(projectId), ct)));
projects.MapPost(string.Empty, async (IMediator mediator, CreateProjectCommand command, CancellationToken ct) =>
    Results.Ok(await mediator.Send(command, ct)));
projects.MapPut("/{projectId:guid}", async (IMediator mediator, Guid projectId, UpdateProjectRequest request, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new UpdateProjectCommand(projectId, request.WorkspaceId, request.OwnerUserId, request.Name), ct)));
projects.MapPost("/{projectId:guid}/archive", async (IMediator mediator, Guid projectId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ArchiveProjectCommand(projectId), ct)));
projects.MapPost("/{projectId:guid}/clone", async (IMediator mediator, Guid projectId, CloneProjectRequest request, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new CloneProjectCommand(projectId, request.Name), ct)));
projects.MapGet("/{projectId:guid}/scope-changes", async (IMediator mediator, Guid projectId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ListScopeChangesQuery(projectId), ct)));
projects.MapGet("/{projectId:guid}/scope-changes/{scopeChangeId:guid}", async (IMediator mediator, Guid projectId, Guid scopeChangeId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetScopeChangeQuery(projectId, scopeChangeId), ct)));
projects.MapPost("/{projectId:guid}/scope-changes", async (IMediator mediator, Guid projectId, RecordScopeChangeRequest request, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new RecordScopeChangeCommand(projectId, request.Title, request.Description, request.AffectedRequirementCitation), ct)));
projects.MapPost("/{projectId:guid}/scope-changes/{scopeChangeId:guid}/approve", async (IMediator mediator, Guid projectId, Guid scopeChangeId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ApproveScopeChangeCommand(projectId, scopeChangeId), ct)));
projects.MapPost("/{projectId:guid}/scope-changes/{scopeChangeId:guid}/reject", async (IMediator mediator, Guid projectId, Guid scopeChangeId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new RejectScopeChangeCommand(projectId, scopeChangeId), ct)));
projects.MapPost("/{projectId:guid}/scope-changes/{scopeChangeId:guid}/implement", async (IMediator mediator, Guid projectId, Guid scopeChangeId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ImplementScopeChangeCommand(projectId, scopeChangeId), ct)));
projects.MapGet("/{projectId:guid}/requirements", async (IMediator mediator, Guid projectId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ListRequirementsQuery(projectId), ct)));
projects.MapPost("/{projectId:guid}/requirements", async (IMediator mediator, Guid projectId, IntakeRequirementRequest request, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new IntakeRequirementCommand(
        projectId,
        request.Title,
        request.Statement,
        request.AcceptanceCriteria,
        request.DocumentTitle,
        request.DocumentContentType,
        request.DocumentUriOrName), ct)));

var requirements = apiV1.MapGroup("/requirements").RequireAuthorization("Bc10Admin");
requirements.MapGet("/{requirementId:guid}", async (IMediator mediator, Guid requirementId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new GetRequirementQuery(requirementId), ct)));
requirements.MapPost("/{requirementId:guid}/approve", async (IMediator mediator, Guid requirementId, CancellationToken ct) =>
    Results.Ok(await mediator.Send(new ApproveRequirementCommand(requirementId), ct)));

using (var scope = app.Services.CreateScope())
{
    var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    if (app.Environment.IsEnvironment("Testing") || !identityDb.Database.IsNpgsql())
    {
        await identityDb.Database.EnsureCreatedAsync();
    }
    else
    {
        await identityDb.Database.MigrateAsync();
    }

    var pluginLoader = scope.ServiceProvider.GetRequiredService<IPluginLoader>();
    await pluginLoader.LoadAsync();
}



if (!app.Environment.IsEnvironment("Testing"))

{

    using var scope = app.Services.CreateScope();

    var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

    var correlationId = Guid.NewGuid();
    var started = new PlatformStartedEvent
    {
        CorrelationId = correlationId,
        CausationId = correlationId,
        ServiceName = "aipm-host"
    };
    await bus.PublishAsync(started);
    await bus.PublishAsync(new PlatformHealthEvent
    {
        CorrelationId = correlationId,
        CausationId = started.MessageId,
        ServiceName = "aipm-host",
        Status = "ready"
    });

}



Log.Information("AIPM host starting — Phase 1 platform skeleton");



app.Run();



/// <summary>Entry point for integration tests.</summary>

public partial class Program;

/// <summary>Permission assignment request.</summary>
public sealed record AssignPermissionRequest(string PermissionCode);

/// <summary>Project update request body.</summary>
public sealed record UpdateProjectRequest(Guid WorkspaceId, Guid OwnerUserId, string Name);

/// <summary>Project clone request body (FR-005).</summary>
public sealed record CloneProjectRequest(string Name);

/// <summary>Scope change record request body (CMD-022).</summary>
public sealed record RecordScopeChangeRequest(string Title, string Description, string? AffectedRequirementCitation);

/// <summary>Requirement intake request body (CMD-030).</summary>
public sealed record IntakeRequirementRequest(
    string Title,
    string Statement,
    IReadOnlyList<string>? AcceptanceCriteria,
    string? DocumentTitle,
    string? DocumentContentType,
    string? DocumentUriOrName);


