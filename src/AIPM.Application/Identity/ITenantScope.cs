using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;

namespace AIPM.Application.Identity;

/// <summary>
/// Resolves tenant scope from the ambient execution context (POL-001).
/// </summary>
public interface ITenantScope
{
    /// <summary>Current tenant when scoped; otherwise null.</summary>
    TenantId? CurrentTenantId { get; }

    /// <summary>True when the request has no tenant in ambient context (platform-scoped).</summary>
    bool IsPlatformScoped { get; }

    /// <summary>Returns the required tenant id or throws when absent.</summary>
    TenantId GetRequiredTenantId();

    /// <summary>Ensures the supplied tenant matches the required ambient tenant scope.</summary>
    void EnsureMatches(Guid tenantId);
}

/// <summary>Execution-context backed tenant scope.</summary>
public sealed class ExecutionContextTenantScope : ITenantScope
{
    private readonly IExecutionContextAccessor _accessor;

    /// <summary>Initializes tenant scope.</summary>
    public ExecutionContextTenantScope(IExecutionContextAccessor accessor)
    {
        _accessor = accessor;
    }

    /// <inheritdoc />
    public TenantId? CurrentTenantId
    {
        get
        {
            var value = _accessor.Current?.TenantId.Value ?? Guid.Empty;
            return value == Guid.Empty ? null : new TenantId(value);
        }
    }

    /// <inheritdoc />
    public bool IsPlatformScoped => CurrentTenantId is null;

    /// <inheritdoc />
    public TenantId GetRequiredTenantId()
        => CurrentTenantId ?? throw new ValidationError("X-Tenant-Id header is required for this operation.");

    /// <inheritdoc />
    public void EnsureMatches(Guid tenantId)
    {
        var current = GetRequiredTenantId();
        if (current.Value != tenantId)
        {
            throw new ForbiddenError("Cross-tenant access is forbidden.");
        }
    }
}
