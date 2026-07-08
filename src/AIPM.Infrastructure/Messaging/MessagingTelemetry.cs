using System.Diagnostics;

namespace AIPM.Infrastructure.Messaging;

/// <summary>
/// Shared tracing source for publish/consume operations.
/// </summary>
public static class MessagingTelemetry
{
    /// <summary>Activity source for messaging operations.</summary>
    public static readonly ActivitySource ActivitySource = new("AIPM.Infrastructure.Messaging");
}
