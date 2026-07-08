using AIPM.Application.Identity;
using AIPM.Application.Identity.Commands;
using AIPM.Application.Identity.Events;
using AIPM.Domain.Identity;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Identity.Repositories;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPM.SharedKernel.Tests.Identity;

/// <summary>
/// BC-10 command handler regression tests (H-09).
/// </summary>
public sealed class IdentityCommandHandlerTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly CapturingEventPublisher _eventPublisher = new();
    private readonly AsyncLocalExecutionContextAccessor _accessor = new();
    private IdentityDbContext _dbContext = null!;
    private TenantRepository _tenantRepository = null!;
    private UserRepository _userRepository = null!;
    private RoleRepository _roleRepository = null!;
    private ExecutionContextTenantScope _tenantScope = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlite(_connection)
            .Options;
        _dbContext = new IdentityDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _tenantRepository = new TenantRepository(_dbContext);
        _userRepository = new UserRepository(_dbContext);
        _roleRepository = new RoleRepository(_dbContext);
        _tenantScope = new ExecutionContextTenantScope(_accessor);
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task ProvisionTenant_DispatchesTenantProvisionedEvent()
    {
        var dispatcher = new IdentityDomainEventDispatcher(_eventPublisher);
        var handler = new ProvisionTenantCommandHandler(_tenantRepository, dispatcher);

        var result = await handler.Handle(new ProvisionTenantCommand("Acme"), CancellationToken.None);

        result.Name.Should().Be("Acme");
        _eventPublisher.Published.Should().ContainSingle();
        _eventPublisher.Published[0].Should().BeOfType<TenantProvisionedIntegrationEvent>();
        ((TenantProvisionedIntegrationEvent)_eventPublisher.Published[0]).Name.Should().Be("Acme");
    }

    [Fact]
    public async Task SuspendTenant_DispatchesTenantSuspendedEvent()
    {
        var dispatcher = new IdentityDomainEventDispatcher(_eventPublisher);
        var provisionHandler = new ProvisionTenantCommandHandler(_tenantRepository, dispatcher);
        var tenant = await provisionHandler.Handle(new ProvisionTenantCommand("Acme"), CancellationToken.None);
        _eventPublisher.Published.Clear();

        var suspendHandler = new SuspendTenantCommandHandler(_tenantRepository, dispatcher, _tenantScope);
        await suspendHandler.Handle(new SuspendTenantCommand(tenant.Id), CancellationToken.None);

        _eventPublisher.Published.Should().ContainSingle();
        _eventPublisher.Published[0].Should().BeOfType<TenantSuspendedIntegrationEvent>();
    }

    [Fact]
    public async Task CreateUser_CrossTenantScope_ThrowsForbidden()
    {
        var tenant = Tenant.Provision("Acme");
        await _tenantRepository.AddAsync(tenant);
        await _tenantRepository.SaveChangesAsync();

        using var scope = PushTenant(tenant.Id);
        var handler = new ManageIdentityAccessCommandsHandler(
            _userRepository,
            _roleRepository,
            _tenantRepository,
            _tenantScope);

        var otherTenantId = Guid.NewGuid();
        var act = async () => await handler.Handle(
            new CreateUserCommand(otherTenantId, "user@other.test"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenError>();
    }

    [Fact]
    public async Task CreateUser_WithoutTenantScope_ThrowsValidationError()
    {
        var tenant = Tenant.Provision("Acme");
        await _tenantRepository.AddAsync(tenant);
        await _tenantRepository.SaveChangesAsync();

        var handler = new ManageIdentityAccessCommandsHandler(
            _userRepository,
            _roleRepository,
            _tenantRepository,
            _tenantScope);

        var act = async () => await handler.Handle(
            new CreateUserCommand(tenant.Id, "user@acme.test"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>()
            .WithMessage("*X-Tenant-Id*");
    }

    [Fact]
    public async Task SuspendTenant_WithoutTenantScope_AllowsPlatformAdminSuspend()
    {
        var dispatcher = new IdentityDomainEventDispatcher(_eventPublisher);
        var provisionHandler = new ProvisionTenantCommandHandler(_tenantRepository, dispatcher);
        var tenant = await provisionHandler.Handle(new ProvisionTenantCommand("Acme"), CancellationToken.None);
        _eventPublisher.Published.Clear();

        var suspendHandler = new SuspendTenantCommandHandler(_tenantRepository, dispatcher, _tenantScope);
        var act = async () => await suspendHandler.Handle(new SuspendTenantCommand(tenant.Id), CancellationToken.None);

        await act.Should().NotThrowAsync();
        _eventPublisher.Published.Should().ContainSingle();
    }

    [Fact]
    public async Task SuspendTenant_WithMismatchedTenantScope_ThrowsForbidden()
    {
        var dispatcher = new IdentityDomainEventDispatcher(_eventPublisher);
        var provisionHandler = new ProvisionTenantCommandHandler(_tenantRepository, dispatcher);
        var tenant = await provisionHandler.Handle(new ProvisionTenantCommand("Acme"), CancellationToken.None);

        using var scope = PushTenant(Guid.NewGuid());
        var suspendHandler = new SuspendTenantCommandHandler(_tenantRepository, dispatcher, _tenantScope);

        var act = async () => await suspendHandler.Handle(new SuspendTenantCommand(tenant.Id), CancellationToken.None);
        await act.Should().ThrowAsync<ForbiddenError>();
    }

    [Fact]
    public async Task AssignRole_CrossTenant_ThrowsForbidden()
    {
        var tenantA = Tenant.Provision("Tenant-A");
        var tenantB = Tenant.Provision("Tenant-B");
        await _tenantRepository.AddAsync(tenantA);
        await _tenantRepository.AddAsync(tenantB);
        var roleA = Role.Create(tenantA.Id, "Admin-A");
        var userB = User.Create(tenantB.Id, "user-b@test");
        await _roleRepository.AddAsync(roleA);
        await _userRepository.AddAsync(userB);
        await _dbContext.SaveChangesAsync();

        using var scope = PushTenant(tenantA.Id);
        var handler = new ManageIdentityAccessCommandsHandler(
            _userRepository,
            _roleRepository,
            _tenantRepository,
            _tenantScope);

        var act = async () => await handler.Handle(
            new AssignRoleCommand(userB.Id, roleA.Id),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenError>()
            .WithMessage("*Cross-tenant*");
    }

    [Fact]
    public async Task AssignRole_WithoutTenantScope_ThrowsValidationError()
    {
        var tenant = Tenant.Provision("Acme");
        var role = Role.Create(tenant.Id, "Admin");
        var user = User.Create(tenant.Id, "user@acme.test");
        await _tenantRepository.AddAsync(tenant);
        await _roleRepository.AddAsync(role);
        await _userRepository.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var handler = new ManageIdentityAccessCommandsHandler(
            _userRepository,
            _roleRepository,
            _tenantRepository,
            _tenantScope);

        var act = async () => await handler.Handle(
            new AssignRoleCommand(user.Id, role.Id),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>()
            .WithMessage("*X-Tenant-Id*");
    }

    private IDisposable PushTenant(Guid tenantId)
    {
        var context = RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId));
        return _accessor.Push(context);
    }

    private sealed class CapturingEventPublisher : IIdentityEventPublisher
    {
        public List<object> Published { get; } = [];

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            Published.Add(message);
            return Task.CompletedTask;
        }
    }
}
