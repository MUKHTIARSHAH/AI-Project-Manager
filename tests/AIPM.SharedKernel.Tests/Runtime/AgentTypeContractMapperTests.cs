using AIPM.Application.Runtime.Agents;
using AIPM.Application.Runtime.Contracts;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Runtime;

/// <summary>
/// Tests for public agent SDK catalog contracts.
/// </summary>
public sealed class AgentTypeContractMapperTests
{
    /// <summary>Mapper preserves registry metadata in contract form.</summary>
    [Fact]
    public void ToContract_MapsDescriptor()
    {
        var descriptor = new AgentDescriptor(
            "echo-agent",
            "EchoAgent",
            "1.0.0",
            ["echo"],
            AgentLifecycleState.Registered);

        var contract = AgentTypeContractMapper.ToContract(descriptor);

        contract.Id.Should().Be("echo-agent");
        contract.Name.Should().Be("EchoAgent");
        contract.Version.Should().Be("1.0.0");
        contract.Capabilities.Should().ContainSingle("echo");
        contract.Lifecycle.Should().Be("Registered");
    }
}
