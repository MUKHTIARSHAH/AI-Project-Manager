using AIPM.Application.Identity;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Queries;

/// <summary>Lists tenant portfolios.</summary>
public sealed record ListPortfoliosQuery : IRequest<IReadOnlyList<PortfolioDto>>;

/// <summary>Gets one tenant portfolio by identifier.</summary>
public sealed record GetPortfolioQuery(Guid PortfolioId) : IRequest<PortfolioDto>;

/// <summary>
/// BC-01 portfolio query handlers.
/// Trace: FR-001, FR-003, AGG-002, ADR-SAD-008.
/// </summary>
public sealed class PortfolioQueryHandler :
    IRequestHandler<ListPortfoliosQuery, IReadOnlyList<PortfolioDto>>,
    IRequestHandler<GetPortfolioQuery, PortfolioDto>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes query handlers.</summary>
    public PortfolioQueryHandler(IPortfolioRepository portfolioRepository, ITenantScope tenantScope)
    {
        _portfolioRepository = portfolioRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PortfolioDto>> Handle(ListPortfoliosQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        return (await _portfolioRepository.ListByTenantAsync(tenantId, cancellationToken))
            .Select(x => new PortfolioDto(x.Id, x.TenantId, x.Name, x.CreatedAt))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<PortfolioDto> Handle(GetPortfolioQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var portfolio = await _portfolioRepository.FindAsync(tenantId, request.PortfolioId, cancellationToken)
            ?? throw new NotFoundError($"Portfolio '{request.PortfolioId}' not found.");

        return new PortfolioDto(portfolio.Id, portfolio.TenantId, portfolio.Name, portfolio.CreatedAt);
    }
}
