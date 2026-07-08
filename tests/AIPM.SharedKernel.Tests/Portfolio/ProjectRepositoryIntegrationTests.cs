using AIPM.Domain.Identity;
using AIPM.Domain.Portfolio;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Identity.Repositories;
using AIPM.Infrastructure.Portfolio.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class ProjectRepositoryIntegrationTests : IAsyncLifetime
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
    public async Task ProjectRepository_PersistsAndLoadsProject()
    {
        var tenant = await SeedTenantHierarchyAsync("Acme");
        var repository = new ProjectRepository(_dbContext);
        var project = ProjectAggregate.Create(
            tenant.TenantId,
            tenant.ProgramId,
            Guid.NewGuid(),
            tenant.OwnerUserId,
            "Core Delivery");

        await repository.AddAsync(project);
        await repository.SaveChangesAsync();
        var loaded = await repository.FindAsync(tenant.TenantId, project.Id);

        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("Core Delivery");
        loaded.ProgramId.Should().Be(tenant.ProgramId);
        loaded.Status.Should().Be(ProjectStatus.Draft);
    }

    [Fact]
    public async Task ProjectRepository_ListByTenantAsync_FiltersProjects()
    {
        var tenantA = await SeedTenantHierarchyAsync("Tenant-A");
        var tenantB = await SeedTenantHierarchyAsync("Tenant-B");
        var repository = new ProjectRepository(_dbContext);
        await repository.AddAsync(ProjectAggregate.Create(
            tenantA.TenantId, tenantA.ProgramId, Guid.NewGuid(), tenantA.OwnerUserId, "A Project"));
        await repository.AddAsync(ProjectAggregate.Create(
            tenantB.TenantId, tenantB.ProgramId, Guid.NewGuid(), tenantB.OwnerUserId, "B Project"));
        await repository.SaveChangesAsync();

        var projectsForA = await repository.ListByTenantAsync(tenantA.TenantId);
        projectsForA.Should().ContainSingle(x => x.Name == "A Project");
    }

    [Fact]
    public async Task ProjectRepository_UpdateAsync_PersistsArchive()
    {
        var tenant = await SeedTenantHierarchyAsync("Archive-Tenant");
        var repository = new ProjectRepository(_dbContext);
        var project = ProjectAggregate.Create(
            tenant.TenantId,
            tenant.ProgramId,
            Guid.NewGuid(),
            tenant.OwnerUserId,
            "Archive Target");
        await repository.AddAsync(project);
        await repository.SaveChangesAsync();

        var loaded = await repository.FindAsync(tenant.TenantId, project.Id);
        loaded!.Archive();
        await repository.UpdateAsync(loaded);
        await repository.SaveChangesAsync();

        var archived = await repository.FindAsync(tenant.TenantId, project.Id);
        archived!.Status.Should().Be(ProjectStatus.Archived);
        archived.ArchivedAt.Should().NotBeNull();
    }

    private async Task<(Guid TenantId, Guid ProgramId, Guid OwnerUserId)> SeedTenantHierarchyAsync(string name)
    {
        var tenantRepository = new TenantRepository(_dbContext);
        var userRepository = new UserRepository(_dbContext);
        var portfolioRepository = new PortfolioRepository(_dbContext);
        var programRepository = new ProgramRepository(_dbContext);

        var tenant = Tenant.Provision(name);
        var user = User.Create(tenant.Id, $"{name.ToLowerInvariant()}@acme.test");
        var portfolio = PortfolioAggregate.Create(tenant.Id, $"{name} Portfolio");
        var program = ProgramAggregate.Create(tenant.Id, portfolio.Id, $"{name} Program");

        await tenantRepository.AddAsync(tenant);
        await userRepository.AddAsync(user);
        await portfolioRepository.AddAsync(portfolio);
        await programRepository.AddAsync(program);
        await _dbContext.SaveChangesAsync();

        return (tenant.Id, program.Id, user.Id);
    }
}
