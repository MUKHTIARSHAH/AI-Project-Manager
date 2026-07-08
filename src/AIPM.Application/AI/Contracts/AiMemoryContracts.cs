namespace AIPM.Application.AI.Contracts;

/// <summary>
/// Memory lookup query contract.
/// </summary>
public sealed record AiMemoryQuery(
    string Namespace,
    string Key);

/// <summary>
/// Memory item contract.
/// </summary>
public sealed record AiMemoryItem(
    string Namespace,
    string Key,
    string Value,
    DateTimeOffset UpdatedAt);
