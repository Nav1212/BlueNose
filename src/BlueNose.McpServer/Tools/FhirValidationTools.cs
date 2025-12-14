using System.ComponentModel;
using BlueNose.Core.Models;
using BlueNose.Core.Services;
using ModelContextProtocol.Server;

namespace BlueNose.McpServer.Tools;

/// <summary>
/// MCP tools for FHIR validation operations
/// </summary>
[McpServerToolType]
public class FhirValidationTools
{
    private readonly IFhirValidationService _validationService;

    public FhirValidationTools(IFhirValidationService validationService)
    {
        _validationService = validationService;
    }

    [McpServerTool]
    [Description("Validates a FHIR resource and returns any validation errors, warnings, or informational messages. Supports both FHIR R4 and R5.")]
    public async Task<ValidationToolResult> ValidateFhirResource(
        [Description("The FHIR resource content as a JSON string")] string resourceJson,
        [Description("Optional profile URL to validate against")] string? profileUrl = null,
        [Description("FHIR version override: 'R4' or 'R5' (uses default from config if not specified)")] string? fhirVersion = null)
    {
        var request = new ValidationRequest
        {
            ResourceContent = resourceJson,
            ContentType = "application/fhir+json",
            ProfileUrl = profileUrl,
            FhirVersionOverride = fhirVersion
        };

        var result = await _validationService.ValidateAsync(request);

        return new ValidationToolResult
        {
            IsValid = result.IsValid,
            ResourceType = result.ResourceType,
            FhirVersion = result.FhirVersion,
            ErrorCount = result.Errors.Count,
            WarningCount = result.Warnings.Count,
            Errors = result.Errors.Select(e => new IssueInfo
            {
                Message = e.Message,
                Location = e.Location,
                Code = e.Code
            }).ToList(),
            Warnings = result.Warnings.Select(w => new IssueInfo
            {
                Message = w.Message,
                Location = w.Location,
                Code = w.Code
            }).ToList(),
            DurationMs = result.DurationMs
        };
    }

    [McpServerTool]
    [Description("Performs quick validation check on a FHIR resource and returns a simple pass/fail result with summary.")]
    public async Task<QuickValidationResult> QuickValidateFhir(
        [Description("The FHIR resource content as a JSON string")] string resourceJson)
    {
        var result = await _validationService.ValidateJsonAsync(resourceJson);

        return new QuickValidationResult
        {
            IsValid = result.IsValid,
            ResourceType = result.ResourceType ?? "Unknown",
            Summary = result.IsValid
                ? $"✓ Valid {result.ResourceType} resource"
                : $"✗ Invalid: {result.Errors.Count} error(s), {result.Warnings.Count} warning(s)",
            FhirVersion = result.FhirVersion ?? _validationService.CurrentFhirVersion
        };
    }

    [McpServerTool]
    [Description("Gets the current FHIR version configured for the server.")]
    public string GetCurrentFhirVersion()
    {
        return _validationService.CurrentFhirVersion;
    }
}

/// <summary>
/// Detailed validation result for MCP tool response
/// </summary>
public class ValidationToolResult
{
    public bool IsValid { get; set; }
    public string? ResourceType { get; set; }
    public string? FhirVersion { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public List<IssueInfo> Errors { get; set; } = new();
    public List<IssueInfo> Warnings { get; set; } = new();
    public long DurationMs { get; set; }
}

/// <summary>
/// Issue information for tool responses
/// </summary>
public class IssueInfo
{
    public string Message { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Code { get; set; }
}

/// <summary>
/// Quick validation result for simple pass/fail checks
/// </summary>
public class QuickValidationResult
{
    public bool IsValid { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string FhirVersion { get; set; } = string.Empty;
}
