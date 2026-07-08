using AIPM.Application.Runtime.Workflow;

namespace AIPM.Workflow.Runtime;

/// <summary>
/// In-memory workflow runtime with pause, resume, and cancel.
/// </summary>
public sealed class WorkflowRuntime : IWorkflowRuntime
{
    private readonly Dictionary<Guid, WorkflowInstance> _instances = [];

    /// <inheritdoc />
    public WorkflowExecutionId Start(string workflowName)
    {
        var id = WorkflowExecutionId.New();
        _instances[id.Value] = new WorkflowInstance(workflowName, WorkflowExecutionState.Idle);
        return id;
    }

    /// <inheritdoc />
    public bool Pause(WorkflowExecutionId id)
    {
        if (!_instances.TryGetValue(id.Value, out var instance) || instance.State != WorkflowExecutionState.Running)
        {
            return false;
        }

        _instances[id.Value] = instance with { State = WorkflowExecutionState.Paused };
        return true;
    }

    /// <inheritdoc />
    public bool Resume(WorkflowExecutionId id)
    {
        if (!_instances.TryGetValue(id.Value, out var instance) || instance.State != WorkflowExecutionState.Paused)
        {
            return false;
        }

        _instances[id.Value] = instance with { State = WorkflowExecutionState.Running };
        return true;
    }

    /// <inheritdoc />
    public bool Cancel(WorkflowExecutionId id)
    {
        if (!_instances.TryGetValue(id.Value, out _))
        {
            return false;
        }

        _instances[id.Value] = _instances[id.Value] with { State = WorkflowExecutionState.Cancelled };
        return true;
    }

    /// <inheritdoc />
    public WorkflowExecutionState? GetState(WorkflowExecutionId id)
        => _instances.TryGetValue(id.Value, out var instance) ? instance.State : null;

    /// <inheritdoc />
    public async Task<WorkflowExecutionResult> ExecuteAsync(
        WorkflowExecutionId id,
        Func<CancellationToken, Task<string>> action,
        CancellationToken cancellationToken)
    {
        if (!_instances.TryGetValue(id.Value, out var instance))
        {
            throw new InvalidOperationException($"Workflow {id.Value} not found.");
        }

        if (instance.State is WorkflowExecutionState.Cancelled)
        {
            return new WorkflowExecutionResult(id, WorkflowExecutionState.Cancelled, null);
        }

        _instances[id.Value] = instance with { State = WorkflowExecutionState.Running };

        try
        {
            var output = await action(cancellationToken);
            _instances[id.Value] = _instances[id.Value] with
            {
                State = WorkflowExecutionState.Completed,
                Output = output
            };
            return new WorkflowExecutionResult(id, WorkflowExecutionState.Completed, output);
        }
        catch (OperationCanceledException)
        {
            _instances[id.Value] = _instances[id.Value] with { State = WorkflowExecutionState.Cancelled };
            throw;
        }
        catch
        {
            _instances[id.Value] = _instances[id.Value] with { State = WorkflowExecutionState.Failed };
            throw;
        }
    }

    private sealed record WorkflowInstance(string Name, WorkflowExecutionState State, string? Output = null);
}
