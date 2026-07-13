using AIPM.SharedKernel.Errors;

namespace AIPM.Domain.Requirements;

/// <summary>
/// Optional source document metadata for FR-010 intake (no binary blob storage).
/// Trace: FR-010, CAP-005, CON-013, AGG-005.
/// </summary>
public sealed class DocumentMetadata
{
    private DocumentMetadata(string? title, string? contentType, string? uriOrName)
    {
        Title = title;
        ContentType = contentType;
        UriOrName = uriOrName;
    }

    /// <summary>Optional document title.</summary>
    public string? Title { get; }

    /// <summary>Optional content type (e.g. application/pdf).</summary>
    public string? ContentType { get; }

    /// <summary>Optional URI or file name reference (not binary content).</summary>
    public string? UriOrName { get; }

    /// <summary>Whether any metadata field is present.</summary>
    public bool HasValue =>
        !string.IsNullOrWhiteSpace(Title)
        || !string.IsNullOrWhiteSpace(ContentType)
        || !string.IsNullOrWhiteSpace(UriOrName);

    /// <summary>Creates optional document metadata; returns null when all fields empty.</summary>
    public static DocumentMetadata? CreateOptional(string? title, string? contentType, string? uriOrName)
    {
        var trimmedTitle = NormalizeOptional(title, 200, "Document title");
        var trimmedContentType = NormalizeOptional(contentType, 128, "Document content type");
        var trimmedUriOrName = NormalizeOptional(uriOrName, 2000, "Document URI or name");

        if (trimmedTitle is null && trimmedContentType is null && trimmedUriOrName is null)
        {
            return null;
        }

        return new DocumentMetadata(trimmedTitle, trimmedContentType, trimmedUriOrName);
    }

    /// <summary>Rehydrates from persistence.</summary>
    public static DocumentMetadata? Rehydrate(string? title, string? contentType, string? uriOrName)
        => CreateOptional(title, contentType, uriOrName);

    private static string? NormalizeOptional(string? value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
        {
            throw new ValidationError($"{fieldName} must be at most {maxLength} characters.");
        }

        return trimmed;
    }
}
