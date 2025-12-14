using BlueNose.Core.Configuration;
using BlueNose.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace BlueNose.FhirTesting;

/// <summary>
/// Test helpers for creating service instances without DI
/// </summary>
public static class TestServiceFactory
{
    /// <summary>
    /// Creates a FhirValidationService configured for R4
    /// </summary>
    public static IFhirValidationService CreateR4ValidationService(bool strictValidation = false)
    {
        var options = Options.Create(new FhirOptions
        {
            Version = "R4",
            StrictValidation = strictValidation
        });

        return new FhirValidationService(options, NullLogger<FhirValidationService>.Instance);
    }

    /// <summary>
    /// Creates a FhirValidationService configured for R5
    /// </summary>
    public static IFhirValidationService CreateR5ValidationService(bool strictValidation = false)
    {
        var options = Options.Create(new FhirOptions
        {
            Version = "R5",
            StrictValidation = strictValidation
        });

        return new FhirValidationService(options, NullLogger<FhirValidationService>.Instance);
    }

    /// <summary>
    /// Creates a FhirParserService configured for R4
    /// </summary>
    public static IFhirParserService CreateR4ParserService()
    {
        var options = Options.Create(new FhirOptions
        {
            Version = "R4"
        });

        return new FhirParserService(options, NullLogger<FhirParserService>.Instance);
    }

    /// <summary>
    /// Creates a FhirParserService configured for R5
    /// </summary>
    public static IFhirParserService CreateR5ParserService()
    {
        var options = Options.Create(new FhirOptions
        {
            Version = "R5"
        });

        return new FhirParserService(options, NullLogger<FhirParserService>.Instance);
    }

    /// <summary>
    /// Creates a validation service with custom options
    /// </summary>
    public static IFhirValidationService CreateValidationService(FhirOptions fhirOptions)
    {
        var options = Options.Create(fhirOptions);
        return new FhirValidationService(options, NullLogger<FhirValidationService>.Instance);
    }

    /// <summary>
    /// Creates a parser service with custom options
    /// </summary>
    public static IFhirParserService CreateParserService(FhirOptions fhirOptions)
    {
        var options = Options.Create(fhirOptions);
        return new FhirParserService(options, NullLogger<FhirParserService>.Instance);
    }
}
