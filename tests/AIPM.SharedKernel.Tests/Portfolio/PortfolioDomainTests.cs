using AIPM.Domain.Portfolio;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class PortfolioDomainTests
{
    [Fact]
    public void Create_RaisesPortfolioCreatedDomainEvent()
    {
        var tenantId = Guid.NewGuid();

        var portfolio = PortfolioAggregate.Create(tenantId, "Platform");

        portfolio.TenantId.Should().Be(tenantId);
        portfolio.Name.Should().Be("Platform");
        portfolio.DomainEvents.Should().ContainSingle(x => x is PortfolioCreatedDomainEvent);
    }
}
