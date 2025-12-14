namespace BlueNose.Core.Models;

/// <summary>
/// Request model for parsing FHIR resources
/// </summary>
public class ParseRequest
{
    /// <summary>
    /// The FHIR resource content (JSON or XML)
    /// </summary>
    public string ResourceContent { get; set; } = string.Empty;

    /// <summary>
    /// Content type: "application/fhir+json" or "application/fhir+xml"
    /// </summary>
    public string ContentType { get; set; } = "application/fhir+json";

    /// <summary>
    /// Override FHIR version for this request (null uses default from config)
    /// </summary>
    public string? FhirVersionOverride { get; set; }
}

/// <summary>
/// Response model for parsed FHIR resources
/// </summary>
public class ParseResult
{
    /// <summary>
    /// Whether parsing was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The resource type (e.g., "Patient", "Observation")
    /// </summary>
    public string? ResourceType { get; set; }

    /// <summary>
    /// Resource ID if present
    /// </summary>
    public string? ResourceId { get; set; }

    /// <summary>
    /// FHIR version used for parsing
    /// </summary>
    public string? FhirVersion { get; set; }

    /// <summary>
    /// Error message if parsing failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Serialized resource in requested format
    /// </summary>
    public string? SerializedResource { get; set; }

    /// <summary>
    /// Key metadata extracted from the resource
    /// </summary>
    public Dictionary<string, object?> Metadata { get; set; } = new();
}
