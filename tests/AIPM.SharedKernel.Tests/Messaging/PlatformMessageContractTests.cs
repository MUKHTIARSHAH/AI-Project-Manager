using AIPM.Infrastructure.Messaging;
using AIPM.Infrastructure.Messaging.Contracts;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Messaging;

/// <summary>
/// Tests for immutable/versioned platform message contracts.
/// </summary>
public sealed class PlatformMessageContractTests
{
    /// <summary>PlatformStarted uses contract v1 and metadata.</summary>
    [Fact]
    public void PlatformStarted_ContainsVersionedMetadata()
    {
        var message = new PlatformStartedEvent();

        message.ContractVersion.Should().Be(PlatformMessageContractVersions.V1);
        message.MessageId.Should().NotBe(Guid.Empty);
        message.CorrelationId.Should().NotBe(Guid.Empty);
        message.CausationId.Should().NotBe(Guid.Empty);
        message.OccurredAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }
}
