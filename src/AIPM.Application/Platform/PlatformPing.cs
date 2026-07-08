using MediatR;

namespace AIPM.Application.Platform;

/// <summary>
/// Platform ping command — infrastructure validation only (no business logic).
/// </summary>
public sealed record PlatformPingCommand : IRequest<PlatformPingResponse>;

/// <summary>
/// Response for platform ping.
/// </summary>
public sealed record PlatformPingResponse(bool Ok, string Message);

/// <summary>
/// Handles platform ping for health of application layer.
/// </summary>
public sealed class PlatformPingHandler : IRequestHandler<PlatformPingCommand, PlatformPingResponse>
{
    /// <inheritdoc />
    public Task<PlatformPingResponse> Handle(PlatformPingCommand request, CancellationToken cancellationToken)
        => Task.FromResult(new PlatformPingResponse(true, "application layer ready"));
}
