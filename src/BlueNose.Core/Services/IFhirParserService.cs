using BlueNose.Core.Models;

namespace BlueNose.Core.Services;

/// <summary>
/// Interface for FHIR parsing services
/// </summary>
public interface IFhirParserService
{
    /// <summary>
    /// Parses a FHIR resource from JSON or XML
    /// </summary>
    /// <param name="request">The parse request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parse result with resource metadata</returns>
    Task<ParseResult> ParseAsync(ParseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses a FHIR resource from JSON string
    /// </summary>
    /// <param name="json">The FHIR resource as JSON</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parse result with resource metadata</returns>
    Task<ParseResult> ParseJsonAsync(string json, CancellationToken cancellationToken = default);

    /// <summary>
    /// Converts a FHIR resource between JSON and XML formats
    /// </summary>
    /// <param name="content">The resource content</param>
    /// <param name="fromFormat">Source format ("json" or "xml")</param>
    /// <param name="toFormat">Target format ("json" or "xml")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Converted resource content</returns>
    Task<string> ConvertFormatAsync(string content, string fromFormat, string toFormat, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current FHIR version being used
    /// </summary>
    string CurrentFhirVersion { get; }
}
