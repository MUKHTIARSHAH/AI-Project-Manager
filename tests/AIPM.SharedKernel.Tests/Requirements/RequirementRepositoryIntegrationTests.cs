using AIPM.Domain.Identity;
using AIPM.Domain.Portfolio;
using AIPM.Domain.Requirements;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Identity.Repositories;
using AIPM.Infrastructure.Portfolio.Repositories;
using AIPM.Infrastructure.Requirements.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPM.SharedKernel.Tests.Requirements;

public sealed class RequirementRepositoryIntegrationTests : IAsyncLifetime
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
    public async Task RequirementRepository_PersistsAndLoadsRequirementWithAcceptanceCriteria()
    {
        var seed = await SeedProjectAsync("Acme");
        var repository = new RequirementRepository(_dbContext);
        var requirement = RequirementAggregate.Intake(
            seed.TenantId,
            seed.ProjectId,
            "Billing",
            "Invoice generation must support monthly cycles.",
            ["Cycle closes on last day", "PDF attachment included"],
            DocumentMetadata.CreateOptional("billing.docx", "application/vnd.openxmlformats", "share/billing.docx"));

        await repository.AddAsync(requirement);
        await repository.SaveChangesAsync();
        var loaded = await repository.FindAsync(seed.TenantId, requirement.Id);

        loaded.Should().NotBeNull();
        loaded!.Title.Should().Be("Billing");
        loaded.Status.Should().Be(RequirementStatus.Draft);
        loaded.Parsed.Should().BeTrue();
        loaded.AcceptanceCriteria.Should().HaveCount(2);
        loaded.DocumentMetadata!.UriOrName.Should().Be("share/billing.docx");
    }

    [Fact]
    public async Task RequirementRepository_UpdateAsync_PersistsStatusChange()
    {
        var seed = await SeedProjectAsync("Acme Update");
        var repository = new RequirementRepository(_dbContext);
        var requirement = RequirementAggregate.Intake(seed.TenantId, seed.ProjectId, "Billing", "Invoice generation must support monthly cycles.");

        await repository.AddAsync(requirement);
        await repository.SaveChangesAsync();

        var loaded = await repository.FindAsync(seed.TenantId, requirement.Id);
        var approved = loaded!.Approve();

        await repository.UpdateAsync(approved);
        await repository.SaveChangesAsync();

        var reloaded = await repository.FindAsync(seed.TenantId, requirement.Id);
        reloaded.Should().NotBeNull();
        reloaded!.Status.Should().Be(RequirementStatus.Approved);
        reloaded.Parsed.Should().BeTrue();
    }

    [Fact]
    public async Task RequirementRepository_ListByProjectAsync_FiltersByTenantAndProject()
    {
        var tenantA = await SeedProjectAsync("Tenant-A");
        var tenantB = await SeedProjectAsync("Tenant-B");
        var repository = new RequirementRepository(_dbContext);
        await repository.AddAsync(RequirementAggregate.Intake(
            tenantA.TenantId, tenantA.ProjectId, "A Req", "A statement"));
        await repository.AddAsync(RequirementAggregate.Intake(
            tenantB.TenantId, tenantB.ProjectId, "B Req", "B statement"));
        await repository.SaveChangesAsync();

        var listed = await repository.ListByProjectAsync(tenantA.TenantId, tenantA.ProjectId);
        listed.Should().ContainSingle(x => x.Title == "A Req");
        listed.Should().NotContain(x => x.Title == "B Req");
    }

    private async Task<(Guid TenantId, Guid ProjectId)> SeedProjectAsync(string name)
    {
        var tenantRepository = new TenantRepository(_dbContext);
        var userRepository = new UserRepository(_dbContext);
        var portfolioRepository = new PortfolioRepository(_dbContext);
        var programRepository = new ProgramRepository(_dbContext);
        var projectRepository = new ProjectRepository(_dbContext);

        var tenant = Tenant.Provision(name);
        var user = User.Create(tenant.Id, $"{name.ToLowerInvariant()}@acme.test");
        var portfolio = PortfolioAggregate.Create(tenant.Id, $"{name} Portfolio");
        var program = ProgramAggregate.Create(tenant.Id, portfolio.Id, $"{name} Program");
        var project = ProjectAggregate.Create(tenant.Id, program.Id, Guid.NewGuid(), user.Id, $"{name} Project");

        await tenantRepository.AddAsync(tenant);
        await userRepository.AddAsync(user);
        await portfolioRepository.AddAsync(portfolio);
        await programRepository.AddAsync(program);
        await projectRepository.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        return (tenant.Id, project.Id);
    }
}
