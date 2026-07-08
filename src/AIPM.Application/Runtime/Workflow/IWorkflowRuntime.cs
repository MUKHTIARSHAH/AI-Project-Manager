namespace AIPM.Application.Runtime.Workflow;

/// <summary>
/// Executes platform workflows with pause, resume, and cancel support.
/// </summary>
public interface IWorkflowRuntime
{
    /// <summary>Starts a new workflow execution.</summary>
    WorkflowExecutionId Start(string workflowName);

    /// <summary>Pauses a running workflow.</summary>
    bool Pause(WorkflowExecutionId id);

    /// <summary>Resumes a paused workflow.</summary>
    bool Resume(WorkflowExecutionId id);

    /// <summary>Cancels a workflow.</summary>
    bool Cancel(WorkflowExecutionId id);

    /// <summary>Gets current execution state.</summary>
    WorkflowExecutionState? GetState(WorkflowExecutionId id);

    /// <summary>Runs a workflow to completion.</summary>
    Task<WorkflowExecutionResult> ExecuteAsync(
        WorkflowExecutionId id,
        Func<CancellationToken, Task<string>> action,
        CancellationToken cancellationToken);
}

/// <summary>Workflow execution identifier.</summary>
public readonly record struct WorkflowExecutionId(Guid Value)
{
    /// <summary>Creates a new workflow execution id.</summary>
    public static WorkflowExecutionId New() => new(Guid.NewGuid());
}

/// <summary>Workflow execution states.</summary>
public enum WorkflowExecutionState
{
    /// <summary>Created, not started.</summary>
    Idle = 0,

    /// <summary>Running.</summary>
    Running = 1,

    /// <summary>Paused.</summary>
    Paused = 2,

    /// <summary>Completed successfully.</summary>
    Completed = 3,

    /// <summary>Failed.</summary>
    Failed = 4,

    /// <summary>Cancelled.</summary>
    Cancelled = 5
}

/// <summary>Workflow execution result.</summary>
public sealed record WorkflowExecutionResult(
    WorkflowExecutionId Id,
    WorkflowExecutionState State,
    string? Output);
