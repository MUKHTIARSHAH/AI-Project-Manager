namespace AIPM.SharedKernel.Errors;

/// <summary>Concurrency or duplicate resource conflict.</summary>
public sealed class ConflictError : DomainError
{
    /// <summary>Initializes conflict error.</summary>
    public ConflictError(string message) : base(message) { }
}

/// <summary>Authentication failure.</summary>
public sealed class UnauthorizedError : DomainError
{
    /// <summary>Initializes unauthorized error.</summary>
    public UnauthorizedError(string message) : base(message) { }
}

/// <summary>Policy denial (fail-closed).</summary>
public sealed class ForbiddenError : DomainError
{
    /// <summary>Initializes forbidden error.</summary>
    public ForbiddenError(string message) : base(message) { }
}

/// <summary>External infrastructure failure.</summary>
public sealed class InfrastructureError : DomainError
{
    /// <summary>Initializes infrastructure error.</summary>
    public InfrastructureError(string message) : base(message) { }

    /// <summary>Initializes infrastructure error with inner exception.</summary>
    public InfrastructureError(string message, Exception inner) : base(message, inner) { }
}

/// <summary>Unexpected internal failure.</summary>
public sealed class InternalError : DomainError
{
    /// <summary>Initializes internal error.</summary>
    public InternalError(string message) : base(message) { }

    /// <summary>Initializes internal error with inner exception.</summary>
    public InternalError(string message, Exception inner) : base(message, inner) { }
}
