using AIPM.Application.Identity;
using AIPM.Application.Portfolio;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Requirements.Queries;

/// <summary>Lists requirements for a tenant project.</summary>
public sealed record ListRequirementsQuery(Guid ProjectId) : IRequest<IReadOnlyList<RequirementListItemDto>>;

/// <summary>Gets one tenant-scoped requirement.</summary>
public sealed record GetRequirementQuery(Guid RequirementId) : IRequest<RequirementDto>;

/// <summary>
/// BC-02 requirement query handlers.
/// Trace: FR-011, AGG-005, ADR-SAD-008, ADR-005.
/// </summary>
public sealed class RequirementQueryHandler :
    IRequestHandler<ListRequirementsQuery, IReadOnlyList<RequirementListItemDto>>,
    IRequestHandler<GetRequirementQuery, RequirementDto>
{
    private readonly IRequirementRepository _requirementRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes query handlers.</summary>
    public RequirementQueryHandler(
        IRequirementRepository requirementRepository,
        IProjectRepository projectRepository,
        ITenantScope tenantScope)
    {
        _requirementRepository = requirementRepository;
        _projectRepository = projectRepository;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RequirementListItemDto>> Handle(
        ListRequirementsQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        _ = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        return (await _requirementRepository.ListByProjectAsync(tenantId, request.ProjectId, cancellationToken))
            .Select(RequirementMapping.ToListItemDto)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<RequirementDto> Handle(GetRequirementQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var requirement = await _requirementRepository.FindAsync(tenantId, request.RequirementId, cancellationToken)
            ?? throw new NotFoundError($"Requirement '{request.RequirementId}' not found.");

        return RequirementMapping.ToDto(requirement);
    }
}
