using BlueNose.Core.Configuration;
using BlueNose.Core.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Task = System.Threading.Tasks.Task;

namespace BlueNose.Core.Services;

/// <summary>
/// FHIR parsing service supporting both R4 and R5 versions
/// </summary>
public class FhirParserService : IFhirParserService
{
    private readonly FhirOptions _options;
    private readonly ILogger<FhirParserService> _logger;

    public FhirParserService(
        IOptions<FhirOptions> options,
        ILogger<FhirParserService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string CurrentFhirVersion => _options.Version;

    public async Task<ParseResult> ParseAsync(ParseRequest request, CancellationToken cancellationToken = default)
    {
        var fhirVersion = request.FhirVersionOverride ?? _options.Version;

        _logger.LogInformation("Parsing resource with FHIR version {FhirVersion}", fhirVersion);

        try
        {
            return await ParseResourceAsync(request, fhirVersion, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Parsing failed with exception");
            return new ParseResult
            {
                Success = false,
                FhirVersion = fhirVersion,
                ErrorMessage = ex.Message
            };
        }
    }

    public Task<ParseResult> ParseJsonAsync(string json, CancellationToken cancellationToken = default)
    {
        var request = new ParseRequest
        {
            ResourceContent = json,
            ContentType = "application/fhir+json"
        };
        return ParseAsync(request, cancellationToken);
    }

    public async Task<string> ConvertFormatAsync(string content, string fromFormat, string toFormat, CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        if (fromFormat.Equals(toFormat, StringComparison.OrdinalIgnoreCase))
        {
            return content;
        }

        var isFromJson = fromFormat.Equals("json", StringComparison.OrdinalIgnoreCase);
        return ConvertFormat(content, isFromJson);
    }

    private async Task<ParseResult> ParseResourceAsync(ParseRequest request, string fhirVersion, CancellationToken cancellationToken)
    {
        await Task.Yield();

        var isJson = request.ContentType.Contains("json", StringComparison.OrdinalIgnoreCase);
        var settings = new ParserSettings { PermissiveParsing = !_options.StrictValidation };

        Resource resource;
        if (isJson)
        {
            var jsonParser = new FhirJsonParser(settings);
            resource = jsonParser.Parse<Resource>(request.ResourceContent);
        }
        else
        {
            var xmlParser = new FhirXmlParser(settings);
            resource = xmlParser.Parse<Resource>(request.ResourceContent);
        }

        var serializer = new FhirJsonSerializer(new SerializerSettings { Pretty = true });
        var serialized = serializer.SerializeToString(resource);

        var metadata = ExtractMetadata(resource);

        _logger.LogInformation("Successfully parsed {FhirVersion} {ResourceType} with ID {ResourceId}", 
            fhirVersion, resource.TypeName, resource.Id);

        return new ParseResult
        {
            Success = true,
            ResourceType = resource.TypeName,
            ResourceId = resource.Id,
            FhirVersion = fhirVersion,
            SerializedResource = serialized,
            Metadata = metadata
        };
    }

    private string ConvertFormat(string content, bool fromJson)
    {
        var settings = new ParserSettings { PermissiveParsing = true };

        if (fromJson)
        {
            var parser = new FhirJsonParser(settings);
            var resource = parser.Parse<Resource>(content);
            var serializer = new FhirXmlSerializer(new SerializerSettings { Pretty = true });
            return serializer.SerializeToString(resource);
        }
        else
        {
            var parser = new FhirXmlParser(settings);
            var resource = parser.Parse<Resource>(content);
            var serializer = new FhirJsonSerializer(new SerializerSettings { Pretty = true });
            return serializer.SerializeToString(resource);
        }
    }

    private Dictionary<string, object?> ExtractMetadata(Resource resource)
    {
        var metadata = new Dictionary<string, object?>
        {
            ["id"] = resource.Id,
            ["resourceType"] = resource.TypeName,
            ["versionId"] = resource.VersionId,
            ["lastUpdated"] = resource.Meta?.LastUpdated?.ToString()
        };

        // Add type-specific metadata
        if (resource is Patient patient)
        {
            metadata["name"] = patient.Name.FirstOrDefault()?.ToString();
            metadata["birthDate"] = patient.BirthDate;
            metadata["gender"] = patient.Gender?.ToString();
        }
        else if (resource is Observation observation)
        {
            metadata["status"] = observation.Status?.ToString();
            metadata["code"] = observation.Code?.Coding?.FirstOrDefault()?.Display;
        }

        return metadata;
    }
}
