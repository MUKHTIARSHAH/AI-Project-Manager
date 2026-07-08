namespace AIPM.Application.AI.Contracts;

/// <summary>
/// Tool declaration for model tool-calling.
/// </summary>
public sealed record AiToolDefinition(
    string Name,
    string Description,
    string InputSchemaJson);

/// <summary>
/// Tool call emitted by model/provider.
/// </summary>
public sealed record AiToolCall(
    string Name,
    string ArgumentsJson,
    string CallId);

/// <summary>
/// Tool execution result contract returned to provider pipeline.
/// </summary>
public sealed record AiToolResult(
    string CallId,
    string OutputJson,
    bool Success);
