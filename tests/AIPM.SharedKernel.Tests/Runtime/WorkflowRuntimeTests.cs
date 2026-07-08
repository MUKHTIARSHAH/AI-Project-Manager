using AIPM.Application.Runtime.Workflow;
using AIPM.Workflow.Runtime;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Runtime;

/// <summary>
/// Workflow runtime lifecycle tests.
/// </summary>
public sealed class WorkflowRuntimeTests
{
    /// <summary>Execute completes with output.</summary>
    [Fact]
    public async Task Execute_CompletesSuccessfully()
    {
        var runtime = new WorkflowRuntime();
        var id = runtime.Start("test");
        var result = await runtime.ExecuteAsync(id, _ => Task.FromResult("done"), CancellationToken.None);
        result.State.Should().Be(WorkflowExecutionState.Completed);
        result.Output.Should().Be("done");
    }

    /// <summary>Pause and resume during execution setup.</summary>
    [Fact]
    public async Task PauseAndResume_TransitionsState()
    {
        var runtime = new WorkflowRuntime();
        var id = runtime.Start("test");
        runtime.GetState(id).Should().Be(WorkflowExecutionState.Idle);
        await runtime.ExecuteAsync(id, async ct =>
        {
            runtime.Pause(id).Should().BeTrue();
            runtime.GetState(id).Should().Be(WorkflowExecutionState.Paused);
            runtime.Resume(id).Should().BeTrue();
            await Task.Delay(10, ct);
            return "ok";
        }, CancellationToken.None);
    }

    /// <summary>Cancel marks workflow cancelled.</summary>
    [Fact]
    public async Task Cancel_SetsCancelledState()
    {
        var runtime = new WorkflowRuntime();
        var id = runtime.Start("test");
        runtime.Cancel(id).Should().BeTrue();
        var result = await runtime.ExecuteAsync(id, _ => Task.FromResult("x"), CancellationToken.None);
        result.State.Should().Be(WorkflowExecutionState.Cancelled);
    }
}
