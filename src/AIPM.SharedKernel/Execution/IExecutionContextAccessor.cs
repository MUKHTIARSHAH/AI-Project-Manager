namespace AIPM.SharedKernel.Execution;

/// <summary>
/// Provides access to the current ambient execution context.
/// </summary>
public interface IExecutionContextAccessor
{
    /// <summary>Current execution context, if set.</summary>
    RuntimeExecutionContext? Current { get; }

    /// <summary>Sets the current execution context for the async flow.</summary>
    IDisposable Push(RuntimeExecutionContext context);
}
