namespace AIPM.SharedKernel.Execution;

/// <summary>
/// Platform user context (not a business User aggregate).
/// </summary>
public sealed record UserContext(string? UserId, string? DisplayName)
{
    /// <summary>Anonymous system context.</summary>
    public static UserContext System { get; } = new("system", "System");
}
