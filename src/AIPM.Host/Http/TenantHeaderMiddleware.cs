using AIPM.Infrastructure.Configuration;
using AIPM.SharedKernel.Errors;
using AIPM.SharedKernel.Execution;
using AIPM.SharedKernel.Ids;
using Microsoft.Extensions.Options;

namespace AIPM.Host.Http;

/// <summary>
/// Establishes tenant-scoped execution context from X-Tenant-Id for BC-10 routes.
/// </summary>
public sealed class TenantHeaderMiddleware
{
    private const string _tenantHeaderName = "X-Tenant-Id";
    private readonly RequestDelegate _next;

    /// <summary>Initializes middleware.</summary>
    public TenantHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>Processes the request.</summary>
    public async Task InvokeAsync(
        HttpContext context,
        IExecutionContextAccessor accessor,
        IOptions<PlatformOptions> platformOptions)
    {
        if (!RequiresTenantScope(context.Request))
        {
            await _next(context);
            return;
        }

        var options = platformOptions.Value;
        TenantId? tenantId = null;

        if (context.Request.Headers.TryGetValue(_tenantHeaderName, out var headerValue)
            && Guid.TryParse(headerValue.ToString(), out var parsedTenantId)
            && parsedTenantId != Guid.Empty)
        {
            tenantId = new TenantId(parsedTenantId);
        }
        else if (options.RequireTenantHeader)
        {
            throw new ValidationError("X-Tenant-Id header is required.");
        }

        if (accessor.Current is not null)
        {
            await _next(context);
            return;
        }

        var executionContext = RuntimeExecutionContext.Create(
            tenantId: tenantId,
            cancellationToken: context.RequestAborted);
        using (accessor.Push(executionContext))
        {
            await _next(context);
        }
    }

    private static bool RequiresTenantScope(HttpRequest request)
    {
        var isIdentityRoute = request.Path.StartsWithSegments("/api/v1/identity", StringComparison.OrdinalIgnoreCase);
        var isPortfolioRoute = request.Path.StartsWithSegments("/api/v1/portfolio", StringComparison.OrdinalIgnoreCase)
            || request.Path.StartsWithSegments("/api/v1/programs", StringComparison.OrdinalIgnoreCase)
            || request.Path.StartsWithSegments("/api/v1/projects", StringComparison.OrdinalIgnoreCase);

        if (!isIdentityRoute && !isPortfolioRoute)
        {
            return false;
        }

        if (isIdentityRoute
            && HttpMethods.IsPost(request.Method)
            && request.Path.Equals("/api/v1/identity/tenants", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }
}
