using AIPM.Application.Identity;
using AIPM.Application.Portfolio.Events;
using AIPM.Domain.Portfolio;
using MediatR;

namespace AIPM.Application.Portfolio.Commands;

/// <summary>
/// CMD-010 CreatePortfolio.
/// Trace: FR-003, AGG-002, EVT-010, ADR-SAD-008.
/// </summary>
public sealed record CreatePortfolioCommand(string Name) : IRequest<PortfolioDto>;

/// <summary>Handles portfolio creation use-case.</summary>
public sealed class CreatePortfolioCommandHandler : IRequestHandler<CreatePortfolioCommand, PortfolioDto>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IPortfolioEventPublisher _portfolioEventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public CreatePortfolioCommandHandler(
        IPortfolioRepository portfolioRepository,
        IPortfolioEventPublisher portfolioEventPublisher,
        ITenantScope tenantScope)
    {
        _portfolioRepository = portfolioRepository;
        _portfolioEventPublisher = portfolioEventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<PortfolioDto> Handle(CreatePortfolioCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var portfolio = PortfolioAggregate.Create(tenantId, request.Name);

        await _portfolioRepository.AddAsync(portfolio, cancellationToken);
        await _portfolioRepository.SaveChangesAsync(cancellationToken);

        await _portfolioEventPublisher.PublishAsync(new PortfolioCreatedIntegrationEvent
        {
            PortfolioId = portfolio.Id,
            TenantId = portfolio.TenantId,
            Name = portfolio.Name
        }, cancellationToken);

        return new PortfolioDto(portfolio.Id, portfolio.TenantId, portfolio.Name, portfolio.CreatedAt);
    }
}
