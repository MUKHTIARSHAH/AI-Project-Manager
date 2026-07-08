namespace AIPM.SharedKernel.Execution;

/// <summary>
/// Request-scoped metadata propagated through the runtime.
/// </summary>
public sealed record RequestMetadata(
    string? Source,
    DateTimeOffset ReceivedAt,
    IReadOnlyDictionary<string, string>? Headers = null);
