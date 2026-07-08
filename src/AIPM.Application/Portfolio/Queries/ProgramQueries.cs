using AIPM.Application.Identity;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Queries;

/// <summary>Lists tenant programs.</summary>
public sealed record ListProgramsQuery : IRequest<IReadOnlyList<ProgramDto>>;

/// <summary>Gets one tenant program.</summary>
public sealed record GetProgramQuery(Guid ProgramId) : IRequest<ProgramDto>;

/// <summary>
/// BC-01 program query handlers.
/// Trace: FR-003, AGG-003, ADR-SAD-008.
/// </summary>
public sealed class ProgramQueryHandler :
    IRequestHandler<ListProgramsQuery, IReadOnlyList<ProgramDto>>,
    IRequestHandler<GetProgramQuery, ProgramDto>
{
    private readonly IProgramRepository _programRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes query handlers.</summary>
    public ProgramQueryHandler(IProgramRepository programRepository, ITenantScope tenantScope)
    {
        _programRepository = programRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProgramDto>> Handle(ListProgramsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        return (await _programRepository.ListByTenantAsync(tenantId, cancellationToken))
            .Select(x => new ProgramDto(x.Id, x.TenantId, x.PortfolioId, x.Name, x.CreatedAt))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<ProgramDto> Handle(GetProgramQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var program = await _programRepository.FindAsync(tenantId, request.ProgramId, cancellationToken)
            ?? throw new NotFoundError($"Program '{request.ProgramId}' not found.");

        return new ProgramDto(program.Id, program.TenantId, program.PortfolioId, program.Name, program.CreatedAt);
    }
}
