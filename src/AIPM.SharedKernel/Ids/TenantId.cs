namespace AIPM.SharedKernel.Ids;

/// <summary>
/// Strongly-typed tenant identifier. Immutable once assigned (ADR-SAD-006).
/// </summary>
public readonly record struct TenantId(Guid Value)
{
    /// <summary>Creates a new tenant identifier.</summary>
    public static TenantId New() => new(Guid.NewGuid());

    /// <summary>Parses a tenant identifier from string.</summary>
    public static TenantId Parse(string value) => new(Guid.Parse(value));

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
