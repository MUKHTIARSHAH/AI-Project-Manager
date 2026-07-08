namespace AIPM.Application.AI.Contracts;

/// <summary>
/// Completion request contract.
/// </summary>
public sealed record AiCompletionRequest(
    string Provider,
    string Model,
    AiPrompt Prompt,
    IReadOnlyDictionary<string, object?>? Options = null);

/// <summary>
/// Completion response contract.
/// </summary>
public sealed record AiCompletionResponse(
    string Provider,
    string Model,
    string Text,
    DateTimeOffset CompletedAt);

/// <summary>
/// Streaming chunk contract.
/// </summary>
public sealed record AiStreamChunk(
    string Provider,
    string Model,
    string Delta,
    bool IsFinal);
