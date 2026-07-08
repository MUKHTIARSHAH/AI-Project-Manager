using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Portfolio.Commands;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class CreateProgramCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesProgram_WhenPortfolioExists()
    {
        var tenantId = Guid.NewGuid();
        var portfolio = PortfolioAggregate.Create(tenantId, "Strategic");
        var portfolioRepository = new InMemoryPortfolioRepository(portfolio);
        var programRepository = new InMemoryProgramRepository();
        var publisher = new CapturingPortfolioPublisher();
        var accessor = new AsyncLocalExecutionContextAccessor();
        using var scope = accessor.Push(RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId)));
        var tenantScope = new ExecutionContextTenantScope(accessor);
        var handler = new CreateProgramCommandHandler(portfolioRepository, programRepository, publisher, tenantScope);

        var result = await handler.Handle(new CreateProgramCommand(portfolio.Id, "Program A"), CancellationToken.None);

        result.PortfolioId.Should().Be(portfolio.Id);
        programRepository.Items.Should().ContainSingle();
        publisher.Messages.Should().ContainSingle();
    }

    private sealed class InMemoryPortfolioRepository : IPortfolioRepository
    {
        private readonly PortfolioAggregate _portfolio;

        public InMemoryPortfolioRepository(PortfolioAggregate portfolio)
        {
            _portfolio = portfolio;
        }

        public Task AddAsync(PortfolioAggregate portfolio, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<PortfolioAggregate?> FindAsync(Guid tenantId, Guid portfolioId, CancellationToken cancellationToken = default)
            => Task.FromResult(_portfolio.TenantId == tenantId && _portfolio.Id == portfolioId ? _portfolio : null);

        public Task<IReadOnlyList<PortfolioAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PortfolioAggregate>>([_portfolio]);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class InMemoryProgramRepository : IProgramRepository
    {
        public List<ProgramAggregate> Items { get; } = [];

        public Task AddAsync(ProgramAggregate program, CancellationToken cancellationToken = default)
        {
            Items.Add(program);
            return Task.CompletedTask;
        }

        public Task<ProgramAggregate?> FindAsync(Guid tenantId, Guid programId, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.FirstOrDefault(x => x.TenantId == tenantId && x.Id == programId));

        public Task<IReadOnlyList<ProgramAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ProgramAggregate>>(Items.Where(x => x.TenantId == tenantId).ToList());

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
