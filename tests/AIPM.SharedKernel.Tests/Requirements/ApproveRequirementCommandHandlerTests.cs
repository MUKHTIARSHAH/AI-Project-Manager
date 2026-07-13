using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Requirements;
using AIPM.Application.Requirements.Commands;
using AIPM.Application.Requirements.Events;
using AIPM.Domain.Portfolio;
using AIPM.Domain.Requirements;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Requirements;

public sealed class ApproveRequirementCommandHandlerTests
{
    [Fact]
    public async Task Approve_UpdatesRequirementStatus_PublishesEvent_AndKeepsParsedIndependentOfUnderReview()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        var requirement = RequirementAggregate.Intake(tenantId, project.Id, "Approve me", "Statement");
        var fixture = CreateFixture(tenantId, project, requirement);

        var result = await fixture.ApproveHandler.Handle(new ApproveRequirementCommand(requirement.Id), CancellationToken.None);

        result.Status.Should().Be(nameof(RequirementStatus.Approved));
        result.Parsed.Should().BeTrue();
        fixture.Requirements.Items.Should().ContainSingle(x => x.Id == requirement.Id && x.Status == RequirementStatus.Approved);
        fixture.Publisher.Messages.Should().ContainSingle(x => x is RequirementApprovedIntegrationEvent);
        var published = fixture.Publisher.Messages.OfType<RequirementApprovedIntegrationEvent>().Single();
        published.Status.Should().Be(nameof(RequirementStatus.Approved));
        published.Parsed.Should().BeTrue();
    }

    [Fact]
    public async Task Approve_ThrowsNotFound_WhenRequirementMissingOrWrongTenant()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        var fixture = CreateFixture(tenantId, project, null);

        var act = () => fixture.ApproveHandler.Handle(new ApproveRequirementCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundError>().WithMessage("*Requirement*");
    }

    [Fact]
    public async Task Approve_Throws_WhenProjectArchived()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        project.Archive();
        var requirement = RequirementAggregate.Intake(tenantId, project.Id, "Approve me", "Statement");
        var fixture = CreateFixture(tenantId, project, requirement);

        var act = () => fixture.ApproveHandler.Handle(new ApproveRequirementCommand(requirement.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*archived*");
    }

    [Fact]
    public async Task Approve_Throws_WhenNotParsed()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        var requirement = RequirementAggregate.Rehydrate(
            Guid.NewGuid(),
            tenantId,
            project.Id,
            "Title",
            "Statement",
            RequirementStatus.Draft,
            parsed: false,
            documentMetadata: null,
            DateTimeOffset.UtcNow);
        var fixture = CreateFixture(tenantId, project, requirement);

        var act = () => fixture.ApproveHandler.Handle(new ApproveRequirementCommand(requirement.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*parsed*");
    }

    [Fact]
    public async Task Approve_Throws_WhenAlreadyApproved_AndPublishesNoEvent()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        var requirement = RequirementAggregate.Rehydrate(
            Guid.NewGuid(),
            tenantId,
            project.Id,
            "Title",
            "Statement",
            RequirementStatus.Approved,
            parsed: true,
            documentMetadata: null,
            DateTimeOffset.UtcNow);
        var fixture = CreateFixture(tenantId, project, requirement);

        var act = () => fixture.ApproveHandler.Handle(new ApproveRequirementCommand(requirement.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*already approved*");
        fixture.Publisher.Messages.Should().BeEmpty();
    }

    [Fact]
    public async Task Approve_Throws_WhenSuperseded_AndPublishesNoEvent()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        var requirement = RequirementAggregate.Rehydrate(
            Guid.NewGuid(),
            tenantId,
            project.Id,
            "Title",
            "Statement",
            RequirementStatus.Superseded,
            parsed: true,
            documentMetadata: null,
            DateTimeOffset.UtcNow);
        var fixture = CreateFixture(tenantId, project, requirement);

        var act = () => fixture.ApproveHandler.Handle(new ApproveRequirementCommand(requirement.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*superseded*");
        fixture.Publisher.Messages.Should().BeEmpty();
    }

    [Fact]
    public async Task Approve_Throws_WhenRetired_AndPublishesNoEvent()
    {
        var tenantId = Guid.NewGuid();
        var project = ProjectAggregate.Create(tenantId, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Host");
        var requirement = RequirementAggregate.Rehydrate(
            Guid.NewGuid(),
            tenantId,
            project.Id,
            "Title",
            "Statement",
            RequirementStatus.Retired,
            parsed: true,
            documentMetadata: null,
            DateTimeOffset.UtcNow);
        var fixture = CreateFixture(tenantId, project, requirement);

        var act = () => fixture.ApproveHandler.Handle(new ApproveRequirementCommand(requirement.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*retired*");
        fixture.Publisher.Messages.Should().BeEmpty();
    }

    private static Fixture CreateFixture(Guid tenantId, ProjectAggregate project, RequirementAggregate? requirement)
    {
        var accessor = new AsyncLocalExecutionContextAccessor();
        var scope = accessor.Push(RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId)));
        var tenantScope = new ExecutionContextTenantScope(accessor);
        var projects = new InMemoryProjectRepository(project);
        var requirements = new InMemoryRequirementRepository(requirement);
        var publisher = new CapturingRequirementsPublisher();

        return new Fixture(
            scope,
            requirements,
            publisher,
            new ApproveRequirementCommandHandler(projects, requirements, publisher, tenantScope));
    }

    private sealed class Fixture(
        IDisposable scope,
        InMemoryRequirementRepository requirements,
        CapturingRequirementsPublisher publisher,
        ApproveRequirementCommandHandler approveHandler)
        : IDisposable
    {
        public InMemoryRequirementRepository Requirements { get; } = requirements;
        public CapturingRequirementsPublisher Publisher { get; } = publisher;
        public ApproveRequirementCommandHandler ApproveHandler { get; } = approveHandler;

        public void Dispose() => scope.Dispose();
    }

    private sealed class InMemoryProjectRepository(ProjectAggregate? project) : IProjectRepository
    {
        public Task AddAsync(ProjectAggregate value, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(ProjectAggregate value, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<ProjectAggregate?> FindAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default)
            => Task.FromResult(project is not null && project.TenantId == tenantId && project.Id == projectId ? project : null);
        public Task<IReadOnlyList<ProjectAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProjectAggregate>>(project is not null && project.TenantId == tenantId ? [project] : []);
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class InMemoryRequirementRepository(RequirementAggregate? requirement) : IRequirementRepository
    {
        public List<RequirementAggregate> Items { get; } = requirement is null ? [] : [requirement];

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

        public Task<IReadOnlyList<RequirementAggregate>> ListByProjectAsync(Guid tenantId, Guid projectId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<RequirementAggregate>>(Items.Where(x => x.TenantId == tenantId && x.ProjectId == projectId).ToList());

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
