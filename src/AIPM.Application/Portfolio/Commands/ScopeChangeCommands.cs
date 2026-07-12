using AIPM.Application.Identity;
using AIPM.Application.Portfolio.Events;
using AIPM.Domain.Portfolio;
using AIPM.SharedKernel.Errors;
using MediatR;

namespace AIPM.Application.Portfolio.Commands;

/// <summary>
/// CMD-022 RecordScopeChange.
/// Trace: FR-004, CAP-004, CON-011, AGG-004, ADR-SAD-008.
/// </summary>
public sealed record RecordScopeChangeCommand(
    Guid ProjectId,
    string Title,
    string Description,
    string? AffectedRequirementCitation) : IRequest<ScopeChangeDto>;

/// <summary>Handles recording a scope change under a project.</summary>
public sealed class RecordScopeChangeCommandHandler : IRequestHandler<RecordScopeChangeCommand, ScopeChangeDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPortfolioEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public RecordScopeChangeCommandHandler(
        IProjectRepository projectRepository,
        IPortfolioEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<ScopeChangeDto> Handle(RecordScopeChangeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        var scopeChange = project.RecordScopeChange(
            request.Title,
            request.Description,
            request.AffectedRequirementCitation);

        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new ScopeChangeRecordedIntegrationEvent
        {
            ScopeChangeId = scopeChange.Id,
            ProjectId = project.Id,
            TenantId = project.TenantId,
            Title = scopeChange.Title,
            Description = scopeChange.Description,
            AffectedRequirementCitation = scopeChange.AffectedRequirementCitation
        }, cancellationToken);

        return ScopeChangeMappings.ToDto(scopeChange);
    }
}

/// <summary>
/// Approves a Proposed scope change (FR-004).
/// Trace: CAP-004, CON-011, AGG-004, ADR-SAD-008.
/// </summary>
public sealed record ApproveScopeChangeCommand(Guid ProjectId, Guid ScopeChangeId) : IRequest<ScopeChangeDto>;

/// <summary>Handles scope change approval.</summary>
public sealed class ApproveScopeChangeCommandHandler : IRequestHandler<ApproveScopeChangeCommand, ScopeChangeDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPortfolioEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public ApproveScopeChangeCommandHandler(
        IProjectRepository projectRepository,
        IPortfolioEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<ScopeChangeDto> Handle(ApproveScopeChangeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        var scopeChange = project.ApproveScopeChange(request.ScopeChangeId);
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new ScopeChangeApprovedIntegrationEvent
        {
            ScopeChangeId = scopeChange.Id,
            ProjectId = project.Id,
            TenantId = project.TenantId,
            Title = scopeChange.Title
        }, cancellationToken);

        return ScopeChangeMappings.ToDto(scopeChange);
    }
}

/// <summary>
/// Rejects a Proposed scope change (FR-004).
/// Trace: CAP-004, CON-011, AGG-004, ADR-SAD-008.
/// </summary>
public sealed record RejectScopeChangeCommand(Guid ProjectId, Guid ScopeChangeId) : IRequest<ScopeChangeDto>;

/// <summary>Handles scope change rejection.</summary>
public sealed class RejectScopeChangeCommandHandler : IRequestHandler<RejectScopeChangeCommand, ScopeChangeDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPortfolioEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public RejectScopeChangeCommandHandler(
        IProjectRepository projectRepository,
        IPortfolioEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<ScopeChangeDto> Handle(RejectScopeChangeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        var scopeChange = project.RejectScopeChange(request.ScopeChangeId);
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new ScopeChangeRejectedIntegrationEvent
        {
            ScopeChangeId = scopeChange.Id,
            ProjectId = project.Id,
            TenantId = project.TenantId,
            Title = scopeChange.Title
        }, cancellationToken);

        return ScopeChangeMappings.ToDto(scopeChange);
    }
}

/// <summary>
/// Marks an Approved scope change as Implemented (CON-011).
/// Trace: FR-004, CAP-004, AGG-004, ADR-SAD-008.
/// </summary>
public sealed record ImplementScopeChangeCommand(Guid ProjectId, Guid ScopeChangeId) : IRequest<ScopeChangeDto>;

/// <summary>Handles marking a scope change implemented.</summary>
public sealed class ImplementScopeChangeCommandHandler : IRequestHandler<ImplementScopeChangeCommand, ScopeChangeDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPortfolioEventPublisher _eventPublisher;
    private readonly ITenantScope _tenantScope;

    /// <summary>Initializes handler.</summary>
    public ImplementScopeChangeCommandHandler(
        IProjectRepository projectRepository,
        IPortfolioEventPublisher eventPublisher,
        ITenantScope tenantScope)
    {
        _projectRepository = projectRepository;
        _eventPublisher = eventPublisher;
        _tenantScope = tenantScope;
    }

    /// <inheritdoc />
    public async Task<ScopeChangeDto> Handle(ImplementScopeChangeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantScope.GetRequiredTenantId().Value;
        var project = await _projectRepository.FindAsync(tenantId, request.ProjectId, cancellationToken)
            ?? throw new NotFoundError($"Project '{request.ProjectId}' not found.");

        var scopeChange = project.MarkScopeChangeImplemented(request.ScopeChangeId);
        await _projectRepository.UpdateAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new ScopeChangeImplementedIntegrationEvent
        {
            ScopeChangeId = scopeChange.Id,
            ProjectId = project.Id,
            TenantId = project.TenantId,
            Title = scopeChange.Title
        }, cancellationToken);

        return ScopeChangeMappings.ToDto(scopeChange);
    }
}

internal static class ScopeChangeMappings
{
    public static ScopeChangeDto ToDto(ScopeChange scopeChange)
        => new(
            scopeChange.Id,
            scopeChange.ProjectId,
            scopeChange.TenantId,
            scopeChange.Title,
            scopeChange.Description,
            scopeChange.AffectedRequirementCitation,
            scopeChange.Status.ToString(),
            scopeChange.RecordedAt,
            scopeChange.DecidedAt);
}
