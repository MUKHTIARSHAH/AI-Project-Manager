using AIPM.Application.Runtime.Workflow;
using Microsoft.Extensions.DependencyInjection;

namespace AIPM.Workflow;

/// <summary>
/// Workflow layer dependency injection extensions.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Registers workflow runtime services.</summary>
    public static IServiceCollection AddWorkflow(this IServiceCollection services)
    {
        services.AddSingleton<IWorkflowRuntime, Runtime.WorkflowRuntime>();
        return services;
    }
}
