using BlueNose.Core.Configuration;
using BlueNose.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlueNose.Core.Extensions;

/// <summary>
/// Extension methods for registering BlueNose.Core services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds BlueNose FHIR core services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Configuration containing FHIR options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBlueNoseCore(this IServiceCollection services, IConfiguration configuration)
    {
        // Register configuration
        services.Configure<FhirOptions>(configuration.GetSection(FhirOptions.SectionName));

        // Register services
        services.AddSingleton<IFhirValidationService, FhirValidationService>();
        services.AddSingleton<IFhirParserService, FhirParserService>();

        return services;
    }

    /// <summary>
    /// Adds BlueNose FHIR core services with custom options
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure FHIR options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBlueNoseCore(this IServiceCollection services, Action<FhirOptions> configureOptions)
    {
        // Register configuration
        services.Configure(configureOptions);

        // Register services
        services.AddSingleton<IFhirValidationService, FhirValidationService>();
        services.AddSingleton<IFhirParserService, FhirParserService>();

        return services;
    }
}
