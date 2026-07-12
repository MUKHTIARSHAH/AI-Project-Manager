using AIPM.Domain.Portfolio;
using AIPM.Infrastructure.Identity.Persistence;
using AIPM.Infrastructure.Identity.Persistence.Models;
using AIPM.Infrastructure.Portfolio.Persistence.Models;
using AIPM.Infrastructure.Portfolio.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class PortfolioProjectionReadRepositoryTests
{
    [Fact]
    public async Task PortfolioAndProgramSummaries_CountProjectsByStatus()
    {
        await using var db = await CreateDbAsync();
        var tenantId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();
        var programA = Guid.NewGuid();
        var programB = Guid.NewGuid();

        await SeedTenantAsync(db, tenantId, "Digital-Tenant");
        db.Portfolios.Add(new PortfolioRecord
        {
            Id = portfolioId,
            TenantId = tenantId,
            Name = "Digital",
            CreatedAt = DateTimeOffset.UtcNow
        });
        db.Programs.AddRange(
            new ProgramRecord
            {
                Id = programA,
                TenantId = tenantId,
                PortfolioId = portfolioId,
                Name = "Prog-A",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new ProgramRecord
            {
                Id = programB,
                TenantId = tenantId,
                PortfolioId = portfolioId,
                Name = "Prog-B",
                CreatedAt = DateTimeOffset.UtcNow
            });
        db.Projects.AddRange(
            CreateProject(tenantId, programA, "P1", nameof(ProjectStatus.Draft)),
            CreateProject(tenantId, programA, "P2", nameof(ProjectStatus.Active)),
            CreateProject(tenantId, programB, "P3", nameof(ProjectStatus.Archived)));
        await db.SaveChangesAsync();

        var repository = new PortfolioProjectionReadRepository(db);

        var portfolio = await repository.GetPortfolioSummaryAsync(tenantId, portfolioId);
        portfolio.Should().NotBeNull();
        portfolio!.ProgramCount.Should().Be(2);
        portfolio.ProjectCount.Should().Be(3);
        portfolio.DraftProjectCount.Should().Be(1);
        portfolio.ActiveProjectCount.Should().Be(1);
        portfolio.ArchivedProjectCount.Should().Be(1);

        var program = await repository.GetProgramSummaryAsync(tenantId, programA);
        program.Should().NotBeNull();
        program!.ProjectCount.Should().Be(2);
        program.DraftProjectCount.Should().Be(1);
        program.ActiveProjectCount.Should().Be(1);
        program.ArchivedProjectCount.Should().Be(0);
    }

    [Fact]
    public async Task ProjectSummary_IncludesScopeChangeCount_AndPortfolioId()
    {
        await using var db = await CreateDbAsync();
        var tenantId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var workspaceId = Guid.NewGuid();

        await SeedTenantAsync(db, tenantId, "Scope-Tenant");
        db.Portfolios.Add(new PortfolioRecord
        {
            Id = portfolioId,
            TenantId = tenantId,
            Name = "PF",
            CreatedAt = DateTimeOffset.UtcNow
        });
        db.Programs.Add(new ProgramRecord
        {
            Id = programId,
            TenantId = tenantId,
            PortfolioId = portfolioId,
            Name = "PRG",
            CreatedAt = DateTimeOffset.UtcNow
        });
        db.Projects.Add(new ProjectRecord
        {
            Id = projectId,
            TenantId = tenantId,
            ProgramId = programId,
            WorkspaceId = workspaceId,
            OwnerUserId = ownerId,
            Name = "Mobile",
            Status = nameof(ProjectStatus.Draft),
            CreatedAt = DateTimeOffset.UtcNow
        });
        db.ScopeChanges.AddRange(
            new ScopeChangeRecord
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                TenantId = tenantId,
                Title = "A",
                Description = "D",
                Status = "Proposed",
                RecordedAt = DateTimeOffset.UtcNow
            },
            new ScopeChangeRecord
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                TenantId = tenantId,
                Title = "B",
                Description = "D",
                Status = "Approved",
                RecordedAt = DateTimeOffset.UtcNow
            });
        await db.SaveChangesAsync();

        var summary = await new PortfolioProjectionReadRepository(db)
            .GetProjectSummaryAsync(tenantId, projectId);

        summary.Should().NotBeNull();
        summary!.PortfolioId.Should().Be(portfolioId);
        summary.ProgramId.Should().Be(programId);
        summary.OwnerUserId.Should().Be(ownerId);
        summary.WorkspaceId.Should().Be(workspaceId);
        summary.ScopeChangeCount.Should().Be(2);
        summary.Status.Should().Be(nameof(ProjectStatus.Draft));
    }

    [Fact]
    public async Task Summaries_AreTenantIsolated()
    {
        await using var db = await CreateDbAsync();
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var portfolioA = Guid.NewGuid();
        var programA = Guid.NewGuid();

        await SeedTenantAsync(db, tenantA, "Tenant-A");
        await SeedTenantAsync(db, tenantB, "Tenant-B");
        db.Portfolios.Add(new PortfolioRecord
        {
            Id = portfolioA,
            TenantId = tenantA,
            Name = "A-Only",
            CreatedAt = DateTimeOffset.UtcNow
        });
        db.Programs.Add(new ProgramRecord
        {
            Id = programA,
            TenantId = tenantA,
            PortfolioId = portfolioA,
            Name = "Prog",
            CreatedAt = DateTimeOffset.UtcNow
        });
        db.Projects.Add(CreateProject(tenantA, programA, "PA", nameof(ProjectStatus.Draft)));
        await db.SaveChangesAsync();

        var repository = new PortfolioProjectionReadRepository(db);

        (await repository.GetPortfolioSummaryAsync(tenantB, portfolioA)).Should().BeNull();
        (await repository.GetProgramSummaryAsync(tenantB, programA)).Should().BeNull();
    }

    private static async Task SeedTenantAsync(IdentityDbContext db, Guid tenantId, string name)
    {
        db.Tenants.Add(new TenantRecord { Id = tenantId, Name = name, Status = "Active" });
        await db.SaveChangesAsync();
    }

    private static ProjectRecord CreateProject(Guid tenantId, Guid programId, string name, string status)
        => new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ProgramId = programId,
            WorkspaceId = Guid.NewGuid(),
            OwnerUserId = Guid.NewGuid(),
            Name = name,
            Status = status,
            CreatedAt = DateTimeOffset.UtcNow
        };

    private static async Task<IdentityDbContext> CreateDbAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlite(connection)
            .Options;
        var db = new IdentityDbContext(options);
        await db.Database.EnsureCreatedAsync();
        return db;
    }
}
