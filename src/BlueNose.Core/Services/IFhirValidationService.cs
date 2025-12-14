using BlueNose.Core.Models;

namespace BlueNose.Core.Services;

/// <summary>
/// Interface for FHIR validation services
/// </summary>
public interface IFhirValidationService
{
    /// <summary>
    /// Validates a FHIR resource
    /// </summary>
    /// <param name="request">The validation request containing the resource</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with any issues found</returns>
    Task<ValidationResult> ValidateAsync(ValidationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a FHIR resource from JSON string
    /// </summary>
    /// <param name="json">The FHIR resource as JSON</param>
    /// <param name="profileUrl">Optional profile URL to validate against</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with any issues found</returns>
    Task<ValidationResult> ValidateJsonAsync(string json, string? profileUrl = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current FHIR version being used
    /// </summary>
    string CurrentFhirVersion { get; }
}
