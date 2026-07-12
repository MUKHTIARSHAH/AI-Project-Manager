using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Portfolio.Commands;
using AIPM.Application.Portfolio.Events;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class ScopeChangeCommandHandlerTests
{
    [Fact]
    public async Task Record_PublishesEvent_WhenTenantScopedProjectExists()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "P1");
        var fixture = CreateFixture(tenantId, project);

        var result = await fixture.RecordHandler.Handle(
            new RecordScopeChangeCommand(project.Id, "Add OAuth2", "Extend login", "REQ-9"),
            CancellationToken.None);

        result.Status.Should().Be(nameof(ScopeChangeStatus.Proposed));
        result.Title.Should().Be("Add OAuth2");
        fixture.Publisher.Messages.Should().ContainSingle(x => x is ScopeChangeRecordedIntegrationEvent);
        fixture.Projects.Items.Single().ScopeChanges.Should().ContainSingle();
    }

    [Fact]
    public async Task Record_ThrowsNotFound_WhenCrossTenantProject()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantA, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "P1");
        var fixture = CreateFixture(tenantB, project);

        var act = () => fixture.RecordHandler.Handle(
            new RecordScopeChangeCommand(project.Id, "Title", "Desc", null),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundError>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Record_Throws_WhenTenantMissing()
    {
        var project = ProjectAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "P1");
        var projects = new InMemoryProjectRepository(project);
        var publisher = new CapturingPortfolioPublisher();
        var accessor = new AsyncLocalExecutionContextAccessor();
        var tenantScope = new ExecutionContextTenantScope(accessor);
        var handler = new RecordScopeChangeCommandHandler(projects, publisher, tenantScope);

        var act = () => handler.Handle(
            new RecordScopeChangeCommand(project.Id, "Title", "Desc", null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*X-Tenant-Id*");
    }

    [Fact]
    public async Task ApproveRejectImplement_PublishTransitionEvents()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "P1");
        var fixture = CreateFixture(tenantId, project);
        var recorded = await fixture.RecordHandler.Handle(
            new RecordScopeChangeCommand(project.Id, "Title", "Desc", null),
            CancellationToken.None);
        fixture.Publisher.Messages.Clear();

        var approved = await fixture.ApproveHandler.Handle(
            new ApproveScopeChangeCommand(project.Id, recorded.Id),
            CancellationToken.None);
        approved.Status.Should().Be(nameof(ScopeChangeStatus.Approved));
        fixture.Publisher.Messages.Should().ContainSingle(x => x is ScopeChangeApprovedIntegrationEvent);

        fixture.Publisher.Messages.Clear();
        var implemented = await fixture.ImplementHandler.Handle(
            new ImplementScopeChangeCommand(project.Id, recorded.Id),
            CancellationToken.None);
        implemented.Status.Should().Be(nameof(ScopeChangeStatus.Implemented));
        fixture.Publisher.Messages.Should().ContainSingle(x => x is ScopeChangeImplementedIntegrationEvent);

        var other = await fixture.RecordHandler.Handle(
            new RecordScopeChangeCommand(project.Id, "Reject Me", "Desc", null),
            CancellationToken.None);
        fixture.Publisher.Messages.Clear();
        var rejected = await fixture.RejectHandler.Handle(
            new RejectScopeChangeCommand(project.Id, other.Id),
            CancellationToken.None);
        rejected.Status.Should().Be(nameof(ScopeChangeStatus.Rejected));
        fixture.Publisher.Messages.Should().ContainSingle(x => x is ScopeChangeRejectedIntegrationEvent);
    }

    private static Fixture CreateFixture(Guid tenantId, ProjectAggregate project)
    {
        var accessor = new AsyncLocalExecutionContextAccessor();
        var scope = accessor.Push(RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId)));
        var tenantScope = new ExecutionContextTenantScope(accessor);
        var projects = new InMemoryProjectRepository(project);
        var publisher = new CapturingPortfolioPublisher();

        return new Fixture(
            scope,
            projects,
            publisher,
            new RecordScopeChangeCommandHandler(projects, publisher, tenantScope),
            new ApproveScopeChangeCommandHandler(projects, publisher, tenantScope),
            new RejectScopeChangeCommandHandler(projects, publisher, tenantScope),
            new ImplementScopeChangeCommandHandler(projects, publisher, tenantScope));
    }

    private sealed class Fixture(
        IDisposable scope,
        InMemoryProjectRepository projects,
        CapturingPortfolioPublisher publisher,
        RecordScopeChangeCommandHandler recordHandler,
        ApproveScopeChangeCommandHandler approveHandler,
        RejectScopeChangeCommandHandler rejectHandler,
        ImplementScopeChangeCommandHandler implementHandler) : IDisposable
    {
        public InMemoryProjectRepository Projects { get; } = projects;
        public CapturingPortfolioPublisher Publisher { get; } = publisher;
        public RecordScopeChangeCommandHandler RecordHandler { get; } = recordHandler;
        public ApproveScopeChangeCommandHandler ApproveHandler { get; } = approveHandler;
        public RejectScopeChangeCommandHandler RejectHandler { get; } = rejectHandler;
        public ImplementScopeChangeCommandHandler ImplementHandler { get; } = implementHandler;

        public void Dispose() => scope.Dispose();
    }

    private sealed class InMemoryProjectRepository(params ProjectAggregate[] seed) : IProjectRepository
    {
        public List<ProjectAggregate> Items { get; } = seed.ToList();

        public Task AddAsync(ProjectAggregate project, CancellationToken cancellationToken = default)
        {
            Items.Add(project);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(ProjectAggregate project, CancellationToken cancellationToken = default)
        {
            var index = Items.FindIndex(x => x.Id == project.Id);
            if (index >= 0)
            {
                Items[index] = project;
            }

            return Task.CompletedTask;
        }

        public Task<ProjectAggregate?> FindAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.FirstOrDefault(x => x.TenantId == tenantId && x.Id == projectId));

        public Task<IReadOnlyList<ProjectAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProjectAggregate>>(Items.Where(x => x.TenantId == tenantId).ToList());

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class CapturingPortfolioPublisher : IPortfolioEventPublisher
    {
        public List<object> Messages { get; } = [];

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
