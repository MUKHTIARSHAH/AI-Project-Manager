using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Portfolio.Queries;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class ProjectionSummaryQueryHandlerTests
{
    [Fact]
    public async Task PortfolioSummary_ReturnsDto_WhenPresent()
    {
        var tenantId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();
        var repository = new InMemoryProjectionRepository();
        repository.AddPortfolio(tenantId, new PortfolioSummaryDto(portfolioId, "PF", 2, 4, 1, 1, 1, 0, 1));
        var handler = CreateHandler(tenantId, repository);

        var result = await handler.Handle(new GetPortfolioSummaryQuery(portfolioId), CancellationToken.None);

        result.PortfolioName.Should().Be("PF");
        result.ProgramCount.Should().Be(2);
        result.ProjectCount.Should().Be(4);
        result.DraftProjectCount.Should().Be(1);
        result.ArchivedProjectCount.Should().Be(1);
    }

    [Fact]
    public async Task ProgramSummary_NotFound_WhenMissing()
    {
        var handler = CreateHandler(Guid.NewGuid(), new InMemoryProjectionRepository());

        var act = () => handler.Handle(new GetProgramSummaryQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundError>().WithMessage("*Program*");
    }

    [Fact]
    public async Task ProjectSummary_RequiresTenant()
    {
        var accessor = new AsyncLocalExecutionContextAccessor();
        var handler = new ProjectionSummaryQueryHandler(
            new InMemoryProjectionRepository(),
            new ExecutionContextTenantScope(accessor));

        var act = () => handler.Handle(new GetProjectSummaryQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationError>().WithMessage("*X-Tenant-Id*");
    }

    [Fact]
    public async Task PortfolioSummary_CrossTenant_ReturnsNotFound()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();
        var repository = new InMemoryProjectionRepository();
        repository.AddPortfolio(tenantA, new PortfolioSummaryDto(portfolioId, "A", 0, 0, 0, 0, 0, 0, 0));
        var handlerB = CreateHandler(tenantB, repository);

        var act = () => handlerB.Handle(new GetPortfolioSummaryQuery(portfolioId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundError>().WithMessage("*Portfolio*");
    }

    private static ProjectionSummaryQueryHandler CreateHandler(
        Guid tenantId,
        InMemoryProjectionRepository repository)
    {
        var accessor = new AsyncLocalExecutionContextAccessor();
        _ = accessor.Push(RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId)));
        return new ProjectionSummaryQueryHandler(repository, new ExecutionContextTenantScope(accessor));
    }

    private sealed class InMemoryProjectionRepository : IPortfolioProjectionReadRepository
    {
        private readonly Dictionary<(Guid TenantId, Guid Id), PortfolioSummaryDto> _portfolios = new();
        private readonly Dictionary<(Guid TenantId, Guid Id), ProgramSummaryDto> _programs = new();
        private readonly Dictionary<(Guid TenantId, Guid Id), ProjectSummaryDto> _projects = new();

        public void AddPortfolio(Guid tenantId, PortfolioSummaryDto dto)
            => _portfolios[(tenantId, dto.PortfolioId)] = dto;

        public Task<PortfolioSummaryDto?> GetPortfolioSummaryAsync(
            Guid tenantId,
            Guid portfolioId,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_portfolios.TryGetValue((tenantId, portfolioId), out var dto) ? dto : null);

        public Task<ProgramSummaryDto?> GetProgramSummaryAsync(
            Guid tenantId,
            Guid programId,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_programs.TryGetValue((tenantId, programId), out var dto) ? dto : null);

        public Task<ProjectSummaryDto?> GetProjectSummaryAsync(
            Guid tenantId,
            Guid projectId,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_projects.TryGetValue((tenantId, projectId), out var dto) ? dto : null);
    }
}
