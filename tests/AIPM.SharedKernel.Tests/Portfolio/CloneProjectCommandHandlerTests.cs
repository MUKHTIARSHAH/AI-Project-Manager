using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Portfolio.Commands;
using AIPM.Application.Portfolio.Events;
using AIPM.Domain.Identity;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class CloneProjectCommandHandlerTests
{
    [Fact]
    public async Task Clone_CreatesNewProject_PublishesClonedEvent_PreservesProgramOwnerWorkspace()
    {
        var tenantId = Guid.NewGuid();
        var program = ProgramAggregate.Create(tenantId, Guid.NewGuid(), "Program A");
        var owner = User.Create(tenantId, "owner@clone.test");
        var fixture = CreateFixture(tenantId, program, owner);
        var workspaceId = Guid.NewGuid();
        var source = await fixture.CreateHandler.Handle(
            new CreateProjectCommand(program.Id, workspaceId, owner.Id, "Source Project"),
            CancellationToken.None);
        fixture.Publisher.Messages.Clear();

        var cloned = await fixture.CloneHandler.Handle(
            new CloneProjectCommand(source.Id, "Cloned Project"),
            CancellationToken.None);

        cloned.SourceProjectId.Should().Be(source.Id);
        cloned.Id.Should().NotBe(source.Id);
        cloned.TenantId.Should().Be(tenantId);
        cloned.ProgramId.Should().Be(program.Id);
        cloned.WorkspaceId.Should().Be(workspaceId);
        cloned.OwnerUserId.Should().Be(owner.Id);
        cloned.Name.Should().Be("Cloned Project");
        cloned.Status.Should().Be(nameof(ProjectStatus.Draft));
        cloned.ArchivedAt.Should().BeNull();
        fixture.Projects.Items.Should().HaveCount(2);
        fixture.Publisher.Messages.Should().ContainSingle(x => x is ProjectClonedIntegrationEvent);
        fixture.Publisher.Messages.Should().NotContain(x => x is ProjectCreatedIntegrationEvent);

        var reloadedSource = await fixture.Projects.FindAsync(tenantId, source.Id);
        reloadedSource!.Name.Should().Be("Source Project");
    }

    [Fact]
    public async Task Clone_DoesNotCopyScopeChanges()
    {
        var tenantId = Guid.NewGuid();
        var program = ProgramAggregate.Create(tenantId, Guid.NewGuid(), "Program A");
        var owner = User.Create(tenantId, "owner@clone.test");
        var fixture = CreateFixture(tenantId, program, owner);
        var source = await fixture.CreateHandler.Handle(
            new CreateProjectCommand(program.Id, Guid.NewGuid(), owner.Id, "With Scope"),
            CancellationToken.None);

        var sourceAgg = (await fixture.Projects.FindAsync(tenantId, source.Id))!;
        sourceAgg.RecordScopeChange("Change", "Body");
        await fixture.Projects.UpdateAsync(sourceAgg);

        var cloned = await fixture.CloneHandler.Handle(
            new CloneProjectCommand(source.Id, "Without Scope"),
            CancellationToken.None);

        var cloneAgg = await fixture.Projects.FindAsync(tenantId, cloned.Id);
        cloneAgg!.ScopeChanges.Should().BeEmpty();
        (await fixture.Projects.FindAsync(tenantId, source.Id))!.ScopeChanges.Should().ContainSingle();
    }

    [Fact]
    public async Task Clone_RequiresTenant()
    {
        var tenantId = Guid.NewGuid();
        var program = ProgramAggregate.Create(tenantId, Guid.NewGuid(), "Program A");
        var owner = User.Create(tenantId, "owner@clone.test");
        var fixture = CreateFixture(tenantId, program, owner);
        fixture.Dispose();

        var act = () => fixture.CloneHandler.Handle(
            new CloneProjectCommand(Guid.NewGuid(), "X"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*X-Tenant-Id*");
    }

    [Fact]
    public async Task Clone_CrossTenantSource_ReturnsNotFound()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var programA = ProgramAggregate.Create(tenantA, Guid.NewGuid(), "Program A");
        var ownerA = User.Create(tenantA, "a@clone.test");
        var fixtureA = CreateFixture(tenantA, programA, ownerA);
        var source = await fixtureA.CreateHandler.Handle(
            new CreateProjectCommand(programA.Id, Guid.NewGuid(), ownerA.Id, "A Source"),
            CancellationToken.None);

        var programB = ProgramAggregate.Create(tenantB, Guid.NewGuid(), "Program B");
        var ownerB = User.Create(tenantB, "b@clone.test");
        var fixtureB = CreateFixture(tenantB, programB, ownerB, sharedProjects: fixtureA.Projects);

        var act = () => fixtureB.CloneHandler.Handle(
            new CloneProjectCommand(source.Id, "Stolen Clone"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundError>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Clone_Archived_Throws()
    {
        var tenantId = Guid.NewGuid();
        var program = ProgramAggregate.Create(tenantId, Guid.NewGuid(), "Program A");
        var owner = User.Create(tenantId, "owner@clone.test");
        var fixture = CreateFixture(tenantId, program, owner);
        var source = await fixture.CreateHandler.Handle(
            new CreateProjectCommand(program.Id, Guid.NewGuid(), owner.Id, "To Archive"),
            CancellationToken.None);
        await fixture.ArchiveHandler.Handle(new ArchiveProjectCommand(source.Id), CancellationToken.None);

        var act = () => fixture.CloneHandler.Handle(
            new CloneProjectCommand(source.Id, "Clone Archived"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*cannot be cloned*");
    }

    [Fact]
    public async Task Clone_DuplicateName_Throws()
    {
        var tenantId = Guid.NewGuid();
        var program = ProgramAggregate.Create(tenantId, Guid.NewGuid(), "Program A");
        var owner = User.Create(tenantId, "owner@clone.test");
        var fixture = CreateFixture(tenantId, program, owner);
        var source = await fixture.CreateHandler.Handle(
            new CreateProjectCommand(program.Id, Guid.NewGuid(), owner.Id, "Alpha"),
            CancellationToken.None);
        await fixture.CreateHandler.Handle(
            new CreateProjectCommand(program.Id, Guid.NewGuid(), owner.Id, "Beta"),
            CancellationToken.None);

        var act = () => fixture.CloneHandler.Handle(
            new CloneProjectCommand(source.Id, "Beta"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*already exists*");
    }

    private static Fixture CreateFixture(
        Guid tenantId,
        ProgramAggregate? program,
        User owner,
        InMemoryProjectRepository? sharedProjects = null)
    {
        var accessor = new AsyncLocalExecutionContextAccessor();
        var scope = accessor.Push(RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId)));
        var tenantScope = new ExecutionContextTenantScope(accessor);
        var programs = new InMemoryProgramRepository(program);
        var projects = sharedProjects ?? new InMemoryProjectRepository();
        var userRepository = new InMemoryUserRepository(owner);
        var publisher = new CapturingPortfolioPublisher();

        return new Fixture(
            scope,
            projects,
            publisher,
            new CreateProjectCommandHandler(programs, projects, userRepository, publisher, tenantScope),
            new ArchiveProjectCommandHandler(projects, publisher, tenantScope),
            new CloneProjectCommandHandler(projects, publisher, tenantScope));
    }

    private sealed class Fixture(
        IDisposable scope,
        InMemoryProjectRepository projects,
        CapturingPortfolioPublisher publisher,
        CreateProjectCommandHandler createHandler,
        ArchiveProjectCommandHandler archiveHandler,
        CloneProjectCommandHandler cloneHandler) : IDisposable
    {
        public InMemoryProjectRepository Projects { get; } = projects;
        public CapturingPortfolioPublisher Publisher { get; } = publisher;
        public CreateProjectCommandHandler CreateHandler { get; } = createHandler;
        public ArchiveProjectCommandHandler ArchiveHandler { get; } = archiveHandler;
        public CloneProjectCommandHandler CloneHandler { get; } = cloneHandler;

        public void Dispose() => scope.Dispose();
    }

    private sealed class InMemoryProgramRepository(ProgramAggregate? program) : IProgramRepository
    {
        public Task AddAsync(ProgramAggregate value, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<ProgramAggregate?> FindAsync(Guid tenantId, Guid programId, CancellationToken cancellationToken = default)
            => Task.FromResult(program is not null && program.TenantId == tenantId && program.Id == programId ? program : null);

        public Task<IReadOnlyList<ProgramAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProgramAggregate>>(
                program is not null && program.TenantId == tenantId ? [program] : []);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class InMemoryProjectRepository : IProjectRepository
    {
        public List<ProjectAggregate> Items { get; } = [];

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

    private sealed class InMemoryUserRepository(params User[] users) : IUserRepository
    {
        private readonly List<User> _users = users.ToList();

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task<User?> FindAsync(Guid userId, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.FirstOrDefault(x => x.Id == userId));

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users);

        public Task<IReadOnlyList<User>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users.Where(x => x.TenantId == tenantId).ToList());

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
