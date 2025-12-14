using BlueNose.Core.Models;
using BlueNose.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlueNose.ValidationWeb.Controllers;

/// <summary>
/// API controller for FHIR validation operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ValidationController : ControllerBase
{
    private readonly IFhirValidationService _validationService;
    private readonly ILogger<ValidationController> _logger;

    public ValidationController(
        IFhirValidationService validationService,
        ILogger<ValidationController> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    /// <summary>
    /// Validates a FHIR resource
    /// </summary>
    /// <param name="request">The validation request containing the resource</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with any issues found</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ValidationResult>> Validate(
        [FromBody] ValidationRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ResourceContent))
        {
            return BadRequest(new { Error = "Resource content is required" });
        }

        _logger.LogInformation("Validating FHIR resource with content type {ContentType}", request.ContentType);

        var result = await _validationService.ValidateAsync(request, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Validates a FHIR resource from raw JSON body
    /// </summary>
    /// <param name="fhirVersion">Optional FHIR version override (R4 or R5)</param>
    /// <param name="profileUrl">Optional profile URL to validate against</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate/json")]
    [Consumes("application/json", "application/fhir+json")]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ValidationResult>> ValidateJson(
        [FromQuery] string? fhirVersion,
        [FromQuery] string? profileUrl,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(json))
        {
            return BadRequest(new { Error = "Request body is required" });
        }

        var request = new ValidationRequest
        {
            ResourceContent = json,
            ContentType = "application/fhir+json",
            FhirVersionOverride = fhirVersion,
            ProfileUrl = profileUrl
        };

        var result = await _validationService.ValidateAsync(request, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Validates a FHIR resource from raw XML body
    /// </summary>
    /// <param name="fhirVersion">Optional FHIR version override (R4 or R5)</param>
    /// <param name="profileUrl">Optional profile URL to validate against</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate/xml")]
    [Consumes("application/xml", "application/fhir+xml")]
    [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ValidationResult>> ValidateXml(
        [FromQuery] string? fhirVersion,
        [FromQuery] string? profileUrl,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var xml = await reader.ReadToEndAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(xml))
        {
            return BadRequest(new { Error = "Request body is required" });
        }

        var request = new ValidationRequest
        {
            ResourceContent = xml,
            ContentType = "application/fhir+xml",
            FhirVersionOverride = fhirVersion,
            ProfileUrl = profileUrl
        };

        var result = await _validationService.ValidateAsync(request, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets the current FHIR version configured for the server
    /// </summary>
    /// <returns>Current FHIR version</returns>
    [HttpGet("version")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult GetVersion()
    {
        return Ok(new
        {
            FhirVersion = _validationService.CurrentFhirVersion,
            SupportedVersions = new[] { "R4", "R5" }
        });
    }
}
