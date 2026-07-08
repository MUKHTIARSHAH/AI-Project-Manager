namespace AIPM.Workflow;

/// <summary>
/// Workflow state identifiers — FSM skeleton per IAD §4.4.
/// </summary>
public enum WorkflowState
{
    /// <summary>Initial state.</summary>
    Idle = 0,

    /// <summary>Workflow running.</summary>
    Running = 1,

    /// <summary>Workflow completed.</summary>
    Completed = 2,

    /// <summary>Workflow failed.</summary>
    Failed = 3
}

/// <summary>
/// Workflow transition trigger.
/// </summary>
public enum WorkflowTrigger
{
    /// <summary>Start workflow.</summary>
    Start = 0,

    /// <summary>Complete workflow.</summary>
    Complete = 1,

    /// <summary>Fail workflow.</summary>
    Fail = 2
}
