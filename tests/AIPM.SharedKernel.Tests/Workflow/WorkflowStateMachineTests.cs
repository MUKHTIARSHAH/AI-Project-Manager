using AIPM.Workflow;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Workflow;

/// <summary>
/// Tests for workflow FSM skeleton.
/// </summary>
public sealed class WorkflowStateMachineTests
{
    /// <summary>Idle to Running on Start.</summary>
    [Fact]
    public void Fire_Start_FromIdle_TransitionsToRunning()
    {
        var fsm = new WorkflowStateMachine();
        fsm.Fire(WorkflowTrigger.Start).Should().BeTrue();
        fsm.State.Should().Be(WorkflowState.Running);
    }

    /// <summary>Running to Completed on Complete.</summary>
    [Fact]
    public void Fire_Complete_FromRunning_TransitionsToCompleted()
    {
        var fsm = new WorkflowStateMachine();
        fsm.Fire(WorkflowTrigger.Start);
        fsm.Fire(WorkflowTrigger.Complete).Should().BeTrue();
        fsm.State.Should().Be(WorkflowState.Completed);
    }

    /// <summary>Invalid transition returns false.</summary>
    [Fact]
    public void Fire_InvalidTransition_ReturnsFalse()
    {
        var fsm = new WorkflowStateMachine();
        fsm.Fire(WorkflowTrigger.Complete).Should().BeFalse();
        fsm.State.Should().Be(WorkflowState.Idle);
    }
}
