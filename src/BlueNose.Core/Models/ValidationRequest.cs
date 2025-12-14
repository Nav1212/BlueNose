namespace BlueNose.Core.Models;

/// <summary>
/// Request model for FHIR resource validation
/// </summary>
public class ValidationRequest
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
    /// Optional profile URL to validate against
    /// </summary>
    public string? ProfileUrl { get; set; }

    /// <summary>
    /// Override FHIR version for this request (null uses default from config)
    /// </summary>
    public string? FhirVersionOverride { get; set; }

    /// <summary>
    /// Whether to use strict validation mode for this request
    /// </summary>
    public bool? StrictValidation { get; set; }

    /// <summary>
    /// Whether content is JSON format
    /// </summary>
    public bool IsJson => ContentType.Contains("json", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Whether content is XML format
    /// </summary>
    public bool IsXml => ContentType.Contains("xml", StringComparison.OrdinalIgnoreCase);
}
