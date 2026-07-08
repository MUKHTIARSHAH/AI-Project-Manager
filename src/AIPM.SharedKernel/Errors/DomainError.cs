namespace AIPM.SharedKernel.Errors;

/// <summary>
/// Base type for domain and application errors.
/// </summary>
public abstract class DomainError : Exception
{
    /// <summary>Initializes a new domain error.</summary>
    protected DomainError(string message) : base(message) { }

    /// <summary>Initializes a new domain error with inner exception.</summary>
    protected DomainError(string message, Exception inner) : base(message, inner) { }
}

/// <summary>Validation failure.</summary>
public sealed class ValidationError : DomainError
{
    /// <summary>Initializes validation error.</summary>
    public ValidationError(string message) : base(message) { }
}

/// <summary>Resource not found.</summary>
public sealed class NotFoundError : DomainError
{
    /// <summary>Initializes not found error.</summary>
    public NotFoundError(string message) : base(message) { }
}
