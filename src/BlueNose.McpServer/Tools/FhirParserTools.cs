using System.ComponentModel;
using BlueNose.Core.Models;
using BlueNose.Core.Services;
using ModelContextProtocol.Server;

namespace BlueNose.McpServer.Tools;

/// <summary>
/// MCP tools for FHIR parsing and transformation operations
/// </summary>
[McpServerToolType]
public class FhirParserTools
{
    private readonly IFhirParserService _parserService;

    public FhirParserTools(IFhirParserService parserService)
    {
        _parserService = parserService;
    }

    [McpServerTool]
    [Description("Parses a FHIR resource and extracts metadata including resource type, ID, and key properties. Supports both FHIR R4 and R5.")]
    public async Task<ParseToolResult> ParseFhirResource(
        [Description("The FHIR resource content as a JSON string")] string resourceJson,
        [Description("FHIR version override: 'R4' or 'R5' (uses default from config if not specified)")] string? fhirVersion = null)
    {
        var request = new ParseRequest
        {
            ResourceContent = resourceJson,
            ContentType = "application/fhir+json",
            FhirVersionOverride = fhirVersion
        };

        var result = await _parserService.ParseAsync(request);

        return new ParseToolResult
        {
            Success = result.Success,
            ResourceType = result.ResourceType,
            ResourceId = result.ResourceId,
            FhirVersion = result.FhirVersion,
            ErrorMessage = result.ErrorMessage,
            Metadata = result.Metadata
        };
    }

    [McpServerTool]
    [Description("Converts a FHIR resource between JSON and XML formats.")]
    public async Task<FormatConversionResult> ConvertFhirFormat(
        [Description("The FHIR resource content")] string resourceContent,
        [Description("Source format: 'json' or 'xml'")] string fromFormat,
        [Description("Target format: 'json' or 'xml'")] string toFormat)
    {
        try
        {
            var converted = await _parserService.ConvertFormatAsync(resourceContent, fromFormat, toFormat);
            return new FormatConversionResult
            {
                Success = true,
                ConvertedContent = converted,
                FromFormat = fromFormat,
                ToFormat = toFormat
            };
        }
        catch (Exception ex)
        {
            return new FormatConversionResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                FromFormat = fromFormat,
                ToFormat = toFormat
            };
        }
    }

    [McpServerTool]
    [Description("Extracts key information from a FHIR resource without full parsing. Quick way to identify resource type and basic metadata.")]
    public async Task<ResourceInfoResult> GetFhirResourceInfo(
        [Description("The FHIR resource content as a JSON string")] string resourceJson)
    {
        var result = await _parserService.ParseJsonAsync(resourceJson);

        if (!result.Success)
        {
            return new ResourceInfoResult
            {
                Success = false,
                ErrorMessage = result.ErrorMessage
            };
        }

        return new ResourceInfoResult
        {
            Success = true,
            ResourceType = result.ResourceType ?? "Unknown",
            ResourceId = result.ResourceId,
            FhirVersion = result.FhirVersion ?? _parserService.CurrentFhirVersion,
            Summary = BuildResourceSummary(result)
        };
    }

    private string BuildResourceSummary(ParseResult result)
    {
        var parts = new List<string>
        {
            $"Type: {result.ResourceType}"
        };

        if (!string.IsNullOrEmpty(result.ResourceId))
        {
            parts.Add($"ID: {result.ResourceId}");
        }

        if (result.Metadata.TryGetValue("name", out var name) && name != null)
        {
            parts.Add($"Name: {name}");
        }

        if (result.Metadata.TryGetValue("status", out var status) && status != null)
        {
            parts.Add($"Status: {status}");
        }

        return string.Join(" | ", parts);
    }
}

/// <summary>
/// Parse result for MCP tool response
/// </summary>
public class ParseToolResult
{
    public bool Success { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? FhirVersion { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object?> Metadata { get; set; } = new();
}

/// <summary>
/// Format conversion result
/// </summary>
public class FormatConversionResult
{
    public bool Success { get; set; }
    public string? ConvertedContent { get; set; }
    public string FromFormat { get; set; } = string.Empty;
    public string ToFormat { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Quick resource info result
/// </summary>
public class ResourceInfoResult
{
    public bool Success { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public string? ResourceId { get; set; }
    public string FhirVersion { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}
