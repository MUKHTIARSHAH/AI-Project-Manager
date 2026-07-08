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

public sealed class ProjectCommandHandlerTests
{
    [Fact]
    public async Task Create_CreatesProject_WhenProgramAndOwnerExist()
    {
        var tenantId = Guid.NewGuid();
        var program = ProgramAggregate.Create(tenantId, Guid.NewGuid(), "Program A");
        var owner = User.Create(tenantId, "owner@acme.test");
        var fixture = CreateFixture(tenantId, program, owner);

        var result = await fixture.CreateHandler.Handle(
            new CreateProjectCommand(program.Id, Guid.NewGuid(), owner.Id, "Project A"),
            CancellationToken.None);

        result.Name.Should().Be("Project A");
        result.Status.Should().Be(nameof(ProjectStatus.Draft));
        fixture.Projects.Items.Should().ContainSingle();
        fixture.Publisher.Messages.Should().ContainSingle(x => x is ProjectCreatedIntegrationEvent);
    }

    [Fact]
    public async Task Create_Throws_WhenProgramMissing()
    {
        var tenantId = Guid.NewGuid();
        var owner = User.Create(tenantId, "owner@acme.test");
        var fixture = CreateFixture(tenantId, program: null, owner);

        var act = () => fixture.CreateHandler.Handle(
            new CreateProjectCommand(Guid.NewGuid(), Guid.NewGuid(), owner.Id, "Project A"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundError>().WithMessage("*Program*");
    }

    [Fact]
    public async Task Update_UpdatesProjectFields()
    {
        var tenantId = Guid.NewGuid();
        var program = ProgramAggregate.Create(tenantId, Guid.NewGuid(), "Program A");
        var owner = User.Create(tenantId, "owner@acme.test");
        var newOwner = User.Create(tenantId, "new-owner@acme.test");
        var fixture = CreateFixture(tenantId, program, owner, newOwner);
        var created = await fixture.CreateHandler.Handle(
            new CreateProjectCommand(program.Id, Guid.NewGuid(), owner.Id, "Project A"),
            CancellationToken.None);
        var workspaceId = Guid.NewGuid();

        var updated = await fixture.UpdateHandler.Handle(
            new UpdateProjectCommand(created.Id, workspaceId, newOwner.Id, "Project B"),
            CancellationToken.None);

        updated.Name.Should().Be("Project B");
        updated.OwnerUserId.Should().Be(newOwner.Id);
        updated.WorkspaceId.Should().Be(workspaceId);
    }

    [Fact]
    public async Task Archive_PublishesArchivedEvent_AndBlocksUpdate()
    {
        var tenantId = Guid.NewGuid();
        var program = ProgramAggregate.Create(tenantId, Guid.NewGuid(), "Program A");
        var owner = User.Create(tenantId, "owner@acme.test");
        var fixture = CreateFixture(tenantId, program, owner);
        var created = await fixture.CreateHandler.Handle(
            new CreateProjectCommand(program.Id, Guid.NewGuid(), owner.Id, "Project A"),
            CancellationToken.None);
        fixture.Publisher.Messages.Clear();

        var archived = await fixture.ArchiveHandler.Handle(
            new ArchiveProjectCommand(created.Id),
            CancellationToken.None);

        archived.Status.Should().Be(nameof(ProjectStatus.Archived));
        fixture.Publisher.Messages.Should().ContainSingle(x => x is ProjectArchivedIntegrationEvent);

        var act = () => fixture.UpdateHandler.Handle(
            new UpdateProjectCommand(created.Id, Guid.NewGuid(), owner.Id, "Nope"),
            CancellationToken.None);
        await act.Should().ThrowAsync<ValidationError>().WithMessage("*read-only*");
    }

    private static Fixture CreateFixture(Guid tenantId, ProgramAggregate? program, params User[] users)
    {
        var accessor = new AsyncLocalExecutionContextAccessor();
        var scope = accessor.Push(RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId)));
        var tenantScope = new ExecutionContextTenantScope(accessor);
        var programs = new InMemoryProgramRepository(program);
        var projects = new InMemoryProjectRepository();
        var userRepository = new InMemoryUserRepository(users);
        var publisher = new CapturingPortfolioPublisher();

        return new Fixture(
            scope,
            projects,
            publisher,
            new CreateProjectCommandHandler(programs, projects, userRepository, publisher, tenantScope),
            new UpdateProjectCommandHandler(projects, userRepository, tenantScope),
            new ArchiveProjectCommandHandler(projects, publisher, tenantScope));
    }

    private sealed class Fixture(
        IDisposable scope,
        InMemoryProjectRepository projects,
        CapturingPortfolioPublisher publisher,
        CreateProjectCommandHandler createHandler,
        UpdateProjectCommandHandler updateHandler,
        ArchiveProjectCommandHandler archiveHandler) : IDisposable
    {
        public InMemoryProjectRepository Projects { get; } = projects;
        public CapturingPortfolioPublisher Publisher { get; } = publisher;
        public CreateProjectCommandHandler CreateHandler { get; } = createHandler;
        public UpdateProjectCommandHandler UpdateHandler { get; } = updateHandler;
        public ArchiveProjectCommandHandler ArchiveHandler { get; } = archiveHandler;

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
