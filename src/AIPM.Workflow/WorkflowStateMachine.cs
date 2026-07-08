namespace AIPM.Workflow;

/// <summary>
/// Finite state machine for workflow orchestration skeleton.
/// </summary>
public sealed class WorkflowStateMachine
{
    private WorkflowState _state = WorkflowState.Idle;

    /// <summary>Current workflow state.</summary>
    public WorkflowState State => _state;

    /// <summary>Applies a trigger and returns whether transition succeeded.</summary>
    public bool Fire(WorkflowTrigger trigger)
    {
        var next = (_state, trigger) switch
        {
            (WorkflowState.Idle, WorkflowTrigger.Start) => WorkflowState.Running,
            (WorkflowState.Running, WorkflowTrigger.Complete) => WorkflowState.Completed,
            (WorkflowState.Running, WorkflowTrigger.Fail) => WorkflowState.Failed,
            _ => _state
        };

        if (next == _state)
        {
            return false;
        }

        _state = next;
        return true;
    }
}
