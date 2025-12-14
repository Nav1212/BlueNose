using BlueNose.Core.Models;
using BlueNose.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlueNose.ValidationWeb.Controllers;

/// <summary>
/// API controller for FHIR parsing operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ParserController : ControllerBase
{
    private readonly IFhirParserService _parserService;
    private readonly ILogger<ParserController> _logger;

    public ParserController(
        IFhirParserService parserService,
        ILogger<ParserController> logger)
    {
        _parserService = parserService;
        _logger = logger;
    }

    /// <summary>
    /// Parses a FHIR resource and extracts metadata
    /// </summary>
    /// <param name="request">The parse request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parse result with metadata</returns>
    [HttpPost("parse")]
    [ProducesResponseType(typeof(ParseResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ParseResult>> Parse(
        [FromBody] ParseRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ResourceContent))
        {
            return BadRequest(new { Error = "Resource content is required" });
        }

        _logger.LogInformation("Parsing FHIR resource with content type {ContentType}", request.ContentType);

        var result = await _parserService.ParseAsync(request, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Converts a FHIR resource between JSON and XML formats
    /// </summary>
    /// <param name="fromFormat">Source format (json or xml)</param>
    /// <param name="toFormat">Target format (json or xml)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Converted resource content</returns>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Convert(
        [FromQuery] string fromFormat,
        [FromQuery] string toFormat,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fromFormat) || string.IsNullOrWhiteSpace(toFormat))
        {
            return BadRequest(new { Error = "Both fromFormat and toFormat query parameters are required" });
        }

        using var reader = new StreamReader(Request.Body);
        var content = await reader.ReadToEndAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            return BadRequest(new { Error = "Request body is required" });
        }

        try
        {
            var converted = await _parserService.ConvertFormatAsync(content, fromFormat, toFormat, cancellationToken);

            var contentType = toFormat.Equals("json", StringComparison.OrdinalIgnoreCase)
                ? "application/fhir+json"
                : "application/fhir+xml";

            return Content(converted, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Format conversion failed");
            return BadRequest(new { Error = ex.Message });
        }
    }
}
