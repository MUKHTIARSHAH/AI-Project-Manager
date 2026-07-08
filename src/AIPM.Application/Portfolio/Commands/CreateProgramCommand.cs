using AIPM.Application.Identity;
using AIPM.Application.Portfolio.Events;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Commands;

/// <summary>
/// CMD-011 CreateProgram.
/// Trace: FR-003, AGG-003, EVT-011, ADR-SAD-008.
/// </summary>
public sealed record CreateProgramCommand(Guid PortfolioId, string Name) : IRequest<ProgramDto>;

/// <summary>Handles program creation.</summary>
public sealed class CreateProgramCommandHandler : IRequestHandler<CreateProgramCommand, ProgramDto>
{
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IProgramRepository _programRepository;
    private readonly IPortfolioEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public CreateProgramCommandHandler(
        IPortfolioRepository portfolioRepository,
        IProgramRepository programRepository,
        IPortfolioEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _portfolioRepository = portfolioRepository;
        _programRepository = programRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<ProgramDto> Handle(CreateProgramCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var portfolio = await _portfolioRepository.FindAsync(tenantId, request.PortfolioId, cancellationToken);
        if (portfolio is null)
        {
            throw new NotFoundError($"Portfolio '{request.PortfolioId}' not found.");
        }

        var program = ProgramAggregate.Create(tenantId, request.PortfolioId, request.Name);
        await _programRepository.AddAsync(program, cancellationToken);
        await _programRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new ProgramCreatedIntegrationEvent
        {
            ProgramId = program.Id,
            TenantId = program.TenantId,
            PortfolioId = program.PortfolioId,
            Name = program.Name
        }, cancellationToken);

        return new ProgramDto(program.Id, program.TenantId, program.PortfolioId, program.Name, program.CreatedAt);
    }
}
