namespace AIPM.Application.AI.Contracts;

/// <summary>
/// Provider-agnostic prompt contract for AI completions.
/// </summary>
public sealed record AiPrompt(
    string System,
    string User,
    IReadOnlyDictionary<string, string>? Metadata = null);
