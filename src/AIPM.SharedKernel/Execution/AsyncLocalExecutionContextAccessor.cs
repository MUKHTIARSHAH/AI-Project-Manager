namespace AIPM.SharedKernel.Execution;

/// <summary>
/// AsyncLocal-backed execution context accessor.
/// </summary>
public sealed class AsyncLocalExecutionContextAccessor : IExecutionContextAccessor
{
    private static readonly AsyncLocal<RuntimeExecutionContext?> _store = new();

    /// <inheritdoc />
    public RuntimeExecutionContext? Current => _store.Value;

    /// <inheritdoc />
    public IDisposable Push(RuntimeExecutionContext context)
    {
        var prior = _store.Value;
        _store.Value = context;
        return new PopScope(prior);
    }

    private sealed class PopScope(RuntimeExecutionContext? prior) : IDisposable
    {
        public void Dispose() => _store.Value = prior;
    }
}
