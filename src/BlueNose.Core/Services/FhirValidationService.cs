using System.Diagnostics;
using BlueNose.Core.Configuration;
using BlueNose.Core.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Task = System.Threading.Tasks.Task;

namespace BlueNose.Core.Services;

/// <summary>
/// FHIR validation service supporting both R4 and R5 versions
/// </summary>
public class FhirValidationService : IFhirValidationService
{
    private readonly FhirOptions _options;
    private readonly ILogger<FhirValidationService> _logger;

    public FhirValidationService(
        IOptions<FhirOptions> options,
        ILogger<FhirValidationService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string CurrentFhirVersion => _options.Version;

    public async Task<ValidationResult> ValidateAsync(ValidationRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var fhirVersion = request.FhirVersionOverride ?? _options.Version;

        _logger.LogInformation("Starting validation with FHIR version {FhirVersion}", fhirVersion);

        try
        {
            return await ValidateResourceAsync(request, fhirVersion, stopwatch, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validation failed with exception");
            stopwatch.Stop();

            return new ValidationResult
            {
                FhirVersion = fhirVersion,
                DurationMs = stopwatch.ElapsedMilliseconds,
                Errors = new List<ValidationIssue>
                {
                    new()
                    {
                        Severity = IssueSeverity.Fatal,
                        Message = ex.Message,
                        Details = ex.StackTrace
                    }
                }
            };
        }
    }

    public Task<ValidationResult> ValidateJsonAsync(string json, string? profileUrl = null, CancellationToken cancellationToken = default)
    {
        var request = new ValidationRequest
        {
            ResourceContent = json,
            ContentType = "application/fhir+json",
            ProfileUrl = profileUrl
        };
        return ValidateAsync(request, cancellationToken);
    }

    private async Task<ValidationResult> ValidateResourceAsync(ValidationRequest request, string fhirVersion, Stopwatch stopwatch, CancellationToken cancellationToken)
    {
        await Task.Yield(); // Allow async context
        
        var settings = new ParserSettings
        {
            PermissiveParsing = !(_options.StrictValidation || (request.StrictValidation ?? false))
        };

        var issues = new List<ValidationIssue>();
        string? resourceType = null;

        try
        {
            Resource resource;
            if (request.IsJson)
            {
                var jsonParser = new FhirJsonParser(settings);
                resource = jsonParser.Parse<Resource>(request.ResourceContent);
            }
            else
            {
                var xmlParser = new FhirXmlParser(settings);
                resource = xmlParser.Parse<Resource>(request.ResourceContent);
            }
            resourceType = resource.TypeName;

            // Basic structural validation passed
            _logger.LogInformation("Successfully parsed {FhirVersion} resource of type {ResourceType}", fhirVersion, resourceType);

            // TODO: Add Firely validator integration for profile validation
            // var validator = new Validator(...);
            // var outcome = validator.Validate(resource);
        }
        catch (FormatException ex)
        {
            issues.Add(new ValidationIssue
            {
                Severity = IssueSeverity.Error,
                Message = ex.Message,
                Code = "invalid"
            });
        }

        stopwatch.Stop();

        var result = new ValidationResult
        {
            ResourceType = resourceType,
            FhirVersion = fhirVersion,
            DurationMs = stopwatch.ElapsedMilliseconds,
            Errors = issues.Where(i => i.Severity >= IssueSeverity.Error).ToList(),
            Warnings = issues.Where(i => i.Severity == IssueSeverity.Warning).ToList(),
            Information = issues.Where(i => i.Severity == IssueSeverity.Information).ToList()
        };

        return result;
    }
}
