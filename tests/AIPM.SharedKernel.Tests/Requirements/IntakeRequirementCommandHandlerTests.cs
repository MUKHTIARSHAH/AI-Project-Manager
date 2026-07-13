using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Requirements;
using AIPM.Application.Requirements.Commands;
using AIPM.Application.Requirements.Events;
using AIPM.Application.Requirements.Queries;
using AIPM.Domain.Portfolio;
using AIPM.Domain.Requirements;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Requirements;

public sealed class IntakeRequirementCommandHandlerTests
{
    [Fact]
    public async Task Intake_CreatesDraftRequirement_PublishesEvent_AndKeepsParsedIndependentOfUnderReview()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        var fixture = CreateFixture(tenantId, project);

        var result = await fixture.IntakeHandler.Handle(
            new IntakeRequirementCommand(
                project.Id,
                "Auth",
                "Users sign in with SSO.",
                ["SSO redirect works"],
                "auth.md",
                "text/markdown",
                "docs/auth.md"),
            CancellationToken.None);

        result.Status.Should().Be(nameof(RequirementStatus.Draft));
        result.Parsed.Should().BeTrue();
        result.AcceptanceCriteria.Should().ContainSingle();
        result.DocumentTitle.Should().Be("auth.md");
        fixture.Requirements.Items.Should().ContainSingle();
        fixture.Publisher.Messages.Should().ContainSingle(x => x is RequirementIntakenIntegrationEvent);
        var published = fixture.Publisher.Messages.OfType<RequirementIntakenIntegrationEvent>().Single();
        published.Status.Should().Be(nameof(RequirementStatus.Draft));
        published.Parsed.Should().BeTrue();
    }

    [Fact]
    public async Task Intake_ThrowsNotFound_WhenProjectMissingOrWrongTenant()
    {
        var tenantId = Guid.NewGuid();
        var fixture = CreateFixture(tenantId, project: null);

        var act = () => fixture.IntakeHandler.Handle(
            new IntakeRequirementCommand(Guid.NewGuid(), "T", "S"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundError>().WithMessage("*Project*");
    }

    [Fact]
    public async Task Intake_Throws_WhenProjectArchived()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Archived Host");
        project.Archive();
        var fixture = CreateFixture(tenantId, project);

        var act = () => fixture.IntakeHandler.Handle(
            new IntakeRequirementCommand(project.Id, "T", "S"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*archived*");
    }

    [Fact]
    public async Task ListAndGet_AreTenantScoped()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        var fixture = CreateFixture(tenantId, project);
        var created = await fixture.IntakeHandler.Handle(
            new IntakeRequirementCommand(project.Id, "R1", "Statement"),
            CancellationToken.None);

        var listed = await fixture.QueryHandler.Handle(new ListRequirementsQuery(project.Id), CancellationToken.None);
        listed.Should().ContainSingle(x => x.Id == created.Id && x.Status == nameof(RequirementStatus.Draft));

        var got = await fixture.QueryHandler.Handle(new GetRequirementQuery(created.Id), CancellationToken.None);
        got.Title.Should().Be("R1");
        got.Parsed.Should().BeTrue();
    }

    private static Fixture CreateFixture(Guid tenantId, ProjectAggregate? project)
    {
        var accessor = new AsyncLocalExecutionContextAccessor();
        var scope = accessor.Push(RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId)));
        var tenantScope = new ExecutionContextTenantScope(accessor);
        var projects = new InMemoryProjectRepository(project);
        var requirements = new InMemoryRequirementRepository();
        var publisher = new CapturingRequirementsPublisher();

        return new Fixture(
            scope,
            requirements,
            publisher,
            new IntakeRequirementCommandHandler(projects, requirements, publisher, tenantScope),
            new RequirementQueryHandler(requirements, projects, tenantScope));
    }

    private sealed class Fixture(
        IDisposable scope,
        InMemoryRequirementRepository requirements,
        CapturingRequirementsPublisher publisher,
        IntakeRequirementCommandHandler intakeHandler,
        RequirementQueryHandler queryHandler) : IDisposable
    {
        public InMemoryRequirementRepository Requirements { get; } = requirements;
        public CapturingRequirementsPublisher Publisher { get; } = publisher;
        public IntakeRequirementCommandHandler IntakeHandler { get; } = intakeHandler;
        public RequirementQueryHandler QueryHandler { get; } = queryHandler;

        public void Dispose() => scope.Dispose();
    }

    private sealed class InMemoryProjectRepository(ProjectAggregate? project) : IProjectRepository
    {
        public Task AddAsync(ProjectAggregate value, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(ProjectAggregate value, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ProjectAggregate?> FindAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default)
            => Task.FromResult(project is not null && project.TenantId == tenantId && project.Id == projectId ? project : null);

        public Task<IReadOnlyList<ProjectAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProjectAggregate>>(
                project is not null && project.TenantId == tenantId ? [project] : []);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class InMemoryRequirementRepository : IRequirementRepository
    {
        public List<RequirementAggregate> Items { get; } = [];

        public Task AddAsync(RequirementAggregate requirement, CancellationToken cancellationToken = default)
        {
            Items.Add(requirement);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(RequirementAggregate requirement, CancellationToken cancellationToken = default)
        {
            var index = Items.FindIndex(x => x.Id == requirement.Id);
            if (index >= 0)
            {
                Items[index] = requirement;
            }

            return Task.CompletedTask;
        }

        public Task<RequirementAggregate?> FindAsync(Guid tenantId, Guid requirementId, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.FirstOrDefault(x => x.TenantId == tenantId && x.Id == requirementId));

        public Task<IReadOnlyList<RequirementAggregate>> ListByProjectAsync(
            Guid tenantId,
            Guid projectId,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<RequirementAggregate>>(
                Items.Where(x => x.TenantId == tenantId && x.ProjectId == projectId).ToList());

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class CapturingRequirementsPublisher : IRequirementsEventPublisher
    {
        public List<object> Messages { get; } = [];

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
