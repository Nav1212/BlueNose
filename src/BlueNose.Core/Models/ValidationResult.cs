namespace BlueNose.Core.Models;

/// <summary>
/// Represents the result of a FHIR validation operation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Whether the validation was successful (no errors)
    /// </summary>
    public bool IsValid => !Errors.Any();

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<ValidationIssue> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<ValidationIssue> Warnings { get; set; } = new();

    /// <summary>
    /// List of informational messages
    /// </summary>
    public List<ValidationIssue> Information { get; set; } = new();

    /// <summary>
    /// The resource type that was validated
    /// </summary>
    public string? ResourceType { get; set; }

    /// <summary>
    /// The FHIR version used for validation
    /// </summary>
    public string? FhirVersion { get; set; }

    /// <summary>
    /// Timestamp of validation
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Duration of validation in milliseconds
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Creates a successful validation result
    /// </summary>
    public static ValidationResult Success(string resourceType, string fhirVersion)
    {
        return new ValidationResult
        {
            ResourceType = resourceType,
            FhirVersion = fhirVersion
        };
    }

    /// <summary>
    /// Creates a failed validation result with errors
    /// </summary>
    public static ValidationResult Failure(string resourceType, string fhirVersion, IEnumerable<ValidationIssue> errors)
    {
        return new ValidationResult
        {
            ResourceType = resourceType,
            FhirVersion = fhirVersion,
            Errors = errors.ToList()
        };
    }
}

/// <summary>
/// Represents a single validation issue
/// </summary>
public class ValidationIssue
{
    /// <summary>
    /// Severity of the issue
    /// </summary>
    public IssueSeverity Severity { get; set; }

    /// <summary>
    /// Human-readable message describing the issue
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// FHIRPath location of the issue in the resource
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Issue code (e.g., from OperationOutcome)
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Additional details or diagnostics
    /// </summary>
    public string? Details { get; set; }
}

/// <summary>
/// Severity levels for validation issues
/// </summary>
public enum IssueSeverity
{
    Information,
    Warning,
    Error,
    Fatal
}
