namespace BlueNose.Core.Configuration;

/// <summary>
/// Configuration options for FHIR processing
/// </summary>
public class FhirOptions
{
    public const string SectionName = "Fhir";

    /// <summary>
    /// FHIR version to use: "R4" or "R5"
    /// </summary>
    public string Version { get; set; } = "R4";

    /// <summary>
    /// Whether to validate resources on parse
    /// </summary>
    public bool ValidateOnParse { get; set; } = true;

    /// <summary>
    /// Base URL for FHIR server (if connecting to external server)
    /// </summary>
    public string? ServerBaseUrl { get; set; }

    /// <summary>
    /// Timeout in seconds for FHIR operations
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to use strict validation mode
    /// </summary>
    public bool StrictValidation { get; set; } = false;

    /// <summary>
    /// Gets whether R5 version is configured
    /// </summary>
    public bool IsR5 => Version.Equals("R5", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether R4 version is configured
    /// </summary>
    public bool IsR4 => Version.Equals("R4", StringComparison.OrdinalIgnoreCase);
}
