using AIPM.Application.Platform;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AIPM.SharedKernel.Tests.Application;

/// <summary>
/// Tests for application layer platform handlers.
/// </summary>
public sealed class PlatformPingHandlerTests
{
    /// <summary>Platform ping returns ready message.</summary>
    [Fact]
    public async Task Handle_ReturnsReadyMessage()
    {
        var services = new ServiceCollection();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlatformPingHandler).Assembly));
        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var response = await mediator.Send(new PlatformPingCommand());

        response.Ok.Should().BeTrue();
        response.Message.Should().Contain("ready");
    }
}
