using AIPM.Domain.Portfolio;
using FluentAssertions;

namespace AIPM.SharedKernel.Tests.Portfolio;

public sealed class ProgramDomainTests
{
    [Fact]
    public void Create_RaisesProgramCreatedDomainEvent()
    {
        var program = ProgramAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), "Transformation");

        program.DomainEvents.Should().ContainSingle(x => x is ProgramCreatedDomainEvent);
    }
}
