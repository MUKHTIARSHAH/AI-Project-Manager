using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.Application.Portfolio.Commands;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class CreatePortfolioCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesPortfolio_ForCurrentTenant()
    {
        var tenantId = Guid.NewGuid();
        var repository = new InMemoryPortfolioRepository();
        var publisher = new CapturingPortfolioPublisher();
        var accessor = new AsyncLocalExecutionContextAccessor();
        using var scope = accessor.Push(RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId)));
        var tenantScope = new ExecutionContextTenantScope(accessor);

        var handler = new CreatePortfolioCommandHandler(repository, publisher, tenantScope);

        var result = await handler.Handle(new CreatePortfolioCommand("Delivery"), CancellationToken.None);

        result.TenantId.Should().Be(tenantId);
        repository.Items.Should().ContainSingle();
        publisher.Messages.Should().ContainSingle();
    }

    private sealed class InMemoryPortfolioRepository : IPortfolioRepository
    {
        public List<PortfolioAggregate> Items { get; } = [];

        public Task AddAsync(PortfolioAggregate portfolio, CancellationToken cancellationToken = default)
        {
            Items.Add(portfolio);
            return Task.CompletedTask;
        }

        public Task<PortfolioAggregate?> FindAsync(Guid tenantId, Guid portfolioId, CancellationToken cancellationToken = default)
            => Task.FromResult(Items.FirstOrDefault(x => x.TenantId == tenantId && x.Id == portfolioId));

        public Task<IReadOnlyList<PortfolioAggregate>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<PortfolioAggregate>>(Items.Where(x => x.TenantId == tenantId).ToList());

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
