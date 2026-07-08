using FluentAssertions;
using NetArchTest.Rules;

namespace AIPM.Architecture.Tests;

/// <summary>
/// NetArchTest rules enforcing Clean Architecture boundaries.
/// </summary>
public sealed class ArchitectureTests
{
    /// <summary>Domain must not reference Infrastructure or Host.</summary>
    [Fact]
    public void Domain_ShouldNotReference_InfrastructureOrHost()
    {
        var result = Types.InAssembly(typeof(AIPM.Domain.Primitives.AggregateRoot).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("AIPM.Infrastructure", "AIPM.Host")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>Plugins must not reference Host or Infrastructure.</summary>
    [Fact]
    public void Plugins_ShouldNotReference_HostOrInfrastructure()
    {
        var result = Types.InAssembly(typeof(AIPM.Plugins.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("AIPM.Host", "AIPM.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>Workflow must not reference Host or Infrastructure.</summary>
    [Fact]
    public void Workflow_ShouldNotReference_HostOrInfrastructure()
    {
        var result = Types.InAssembly(typeof(AIPM.Workflow.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("AIPM.Host", "AIPM.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    /// <summary>Application must not reference Host.</summary>
    [Fact]
    public void Application_ShouldNotReference_Host()
    {
        var result = Types.InAssembly(typeof(AIPM.Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOn("AIPM.Host")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            string.Join(", ", result.FailingTypeNames ?? []));
    }
}
