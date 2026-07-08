using AIPM.Application.Identity;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Identity;

/// <summary>
/// Unit tests for tenant scope fail-closed behavior.
/// </summary>
public sealed class TenantScopeTests
{
    private readonly AsyncLocalExecutionContextAccessor _accessor = new();
    private readonly ExecutionContextTenantScope _tenantScope;

    /// <summary>Creates tenant scope tests.</summary>
    public TenantScopeTests()
    {
        _tenantScope = new ExecutionContextTenantScope(_accessor);
    }

    [Fact]
    public void EnsureMatches_WithoutTenantContext_ThrowsValidationError()
    {
        var act = () => _tenantScope.EnsureMatches(Guid.NewGuid());
        act.Should().Throw<ValidationError>()
            .WithMessage("*X-Tenant-Id*");
    }

    [Fact]
    public void EnsureMatches_WithMismatchedTenant_ThrowsForbidden()
    {
        var tenantId = Guid.NewGuid();
        using var scope = PushTenant(tenantId);

        var act = () => _tenantScope.EnsureMatches(Guid.NewGuid());
        act.Should().Throw<ForbiddenError>();
    }

    [Fact]
    public void EnsureMatches_WithMatchingTenant_Succeeds()
    {
        var tenantId = Guid.NewGuid();
        using var scope = PushTenant(tenantId);

        var act = () => _tenantScope.EnsureMatches(tenantId);
        act.Should().NotThrow();
    }

    [Fact]
    public void IsPlatformScoped_IsTrue_WhenTenantAbsent()
    {
        _tenantScope.IsPlatformScoped.Should().BeTrue();
        _tenantScope.CurrentTenantId.Should().BeNull();
    }

    [Fact]
    public void IsPlatformScoped_IsFalse_WhenTenantPresent()
    {
        using var scope = PushTenant(Guid.NewGuid());
        _tenantScope.IsPlatformScoped.Should().BeFalse();
    }

    private IDisposable PushTenant(Guid tenantId)
    {
        var context = RuntimeExecutionContext.Create(tenantId: new TenantId(tenantId));
        return _accessor.Push(context);
    }
}
