namespace AIPM.SharedKernel.Execution;

/// <summary>
/// Strongly-typed correlation identifier for distributed tracing.
/// </summary>
public readonly record struct CorrelationId(Guid Value)
{
    /// <summary>Creates a new correlation identifier.</summary>
    public static CorrelationId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
