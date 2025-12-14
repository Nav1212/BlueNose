using System.ComponentModel;
using BlueNose.Core.Models;
using BlueNose.Core.Services;
using ModelContextProtocol.Server;

namespace BlueNose.ValidationWeb.Tools;

/// <summary>
/// MCP tools for FHIR validation operations (exposed via SSE transport)
/// </summary>
[McpServerToolType]
public class WebFhirValidationTools
{
    private readonly IFhirValidationService _validationService;
    private readonly IFhirParserService _parserService;

    public WebFhirValidationTools(
        IFhirValidationService validationService,
        IFhirParserService parserService)
    {
        _validationService = validationService;
        _parserService = parserService;
    }

    [McpServerTool]
    [Description("Validates a FHIR resource and returns validation results. Supports both FHIR R4 and R5.")]
    public async Task<WebValidationResult> ValidateFhirResource(
        [Description("The FHIR resource content as a JSON string")] string resourceJson,
        [Description("Optional profile URL to validate against")] string? profileUrl = null,
        [Description("FHIR version override: 'R4' or 'R5'")] string? fhirVersion = null)
    {
        var request = new ValidationRequest
        {
            ResourceContent = resourceJson,
            ContentType = "application/fhir+json",
            ProfileUrl = profileUrl,
            FhirVersionOverride = fhirVersion
        };

        var result = await _validationService.ValidateAsync(request);

        return new WebValidationResult
        {
            IsValid = result.IsValid,
            ResourceType = result.ResourceType,
            FhirVersion = result.FhirVersion,
            ErrorCount = result.Errors.Count,
            WarningCount = result.Warnings.Count,
            Errors = result.Errors.Select(e => e.Message).ToList(),
            Warnings = result.Warnings.Select(w => w.Message).ToList()
        };
    }

    [McpServerTool]
    [Description("Parses a FHIR resource and returns metadata including resource type and key properties.")]
    public async Task<WebParseResult> ParseFhirResource(
        [Description("The FHIR resource content as a JSON string")] string resourceJson)
    {
        var result = await _parserService.ParseJsonAsync(resourceJson);

        return new WebParseResult
        {
            Success = result.Success,
            ResourceType = result.ResourceType,
            ResourceId = result.ResourceId,
            FhirVersion = result.FhirVersion,
            ErrorMessage = result.ErrorMessage
        };
    }

    [McpServerTool]
    [Description("Gets information about the current FHIR configuration.")]
    public FhirConfigInfo GetFhirConfig()
    {
        return new FhirConfigInfo
        {
            CurrentVersion = _validationService.CurrentFhirVersion,
            SupportedVersions = new[] { "R4", "R5" },
            Endpoint = "/api/validation"
        };
    }
}

public class WebValidationResult
{
    public bool IsValid { get; set; }
    public string? ResourceType { get; set; }
    public string? FhirVersion { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class WebParseResult
{
    public bool Success { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? FhirVersion { get; set; }
    public string? ErrorMessage { get; set; }
}

public class FhirConfigInfo
{
    public string CurrentVersion { get; set; } = string.Empty;
    public string[] SupportedVersions { get; set; } = Array.Empty<string>();
    public string Endpoint { get; set; } = string.Empty;
}
