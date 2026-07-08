using AIPM.Domain.Identity;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Identity.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPM.SharedKernel.Tests.Identity;

public sealed class IdentityRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private IdentityDbContext _dbContext = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlite(_connection)
            .Options;
        _dbContext = new IdentityDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task TenantRepository_PersistsAndLoadsTenant()
    {
        var repository = new TenantRepository(_dbContext);
        var tenant = Tenant.Provision("Acme");

        await repository.AddAsync(tenant);
        await repository.SaveChangesAsync();
        var loaded = await repository.FindAsync(tenant.Id);

        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("Acme");
    }

    [Fact]
    public async Task UserRepository_PersistsRoleAssignments()
    {
        var tenantRepository = new TenantRepository(_dbContext);
        var userRepository = new UserRepository(_dbContext);
        var roleRepository = new RoleRepository(_dbContext);
        var tenant = Tenant.Provision("Acme");
        var role = Role.Create(tenant.Id, "Approver");
        var user = User.Create(tenant.Id, "user@acme.test");

        await tenantRepository.AddAsync(tenant);
        await roleRepository.AddAsync(role);
        await userRepository.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        user.AssignRole(role.Id);
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();

        var loaded = await userRepository.FindAsync(user.Id);
        loaded!.RoleAssignments.Should().ContainSingle(x => x.RoleId == role.Id);
    }

    [Fact]
    public async Task UserRepository_ListByTenantAsync_FiltersUsers()
    {
        var tenantRepository = new TenantRepository(_dbContext);
        var userRepository = new UserRepository(_dbContext);
        var tenantA = Tenant.Provision("Tenant-A");
        var tenantB = Tenant.Provision("Tenant-B");
        var userA = User.Create(tenantA.Id, "a@tenant-a.test");
        var userB = User.Create(tenantB.Id, "b@tenant-b.test");

        await tenantRepository.AddAsync(tenantA);
        await tenantRepository.AddAsync(tenantB);
        await userRepository.AddAsync(userA);
        await userRepository.AddAsync(userB);
        await _dbContext.SaveChangesAsync();

        var usersForA = await userRepository.ListByTenantAsync(tenantA.Id);
        usersForA.Should().ContainSingle(x => x.Email == "a@tenant-a.test");
    }
}
