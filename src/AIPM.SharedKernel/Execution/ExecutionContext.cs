using AIPM.SharedKernel.Ids;

namespace AIPM.SharedKernel.Execution;

/// <summary>
/// Ambient execution context for platform runtime operations.
/// </summary>
public sealed record RuntimeExecutionContext(
    CorrelationId CorrelationId,
    TenantId TenantId,
    UserContext User,
    RequestMetadata Metadata,
    CancellationToken CancellationToken)
{
    /// <summary>Creates a new execution context with defaults.</summary>
    public static RuntimeExecutionContext Create(
        TenantId? tenantId = null,
        UserContext? user = null,
        CancellationToken cancellationToken = default)
        => new(
            CorrelationId.New(),
            tenantId ?? TenantId.New(),
            user ?? UserContext.System,
            new RequestMetadata("platform", DateTimeOffset.UtcNow),
            cancellationToken);
}
