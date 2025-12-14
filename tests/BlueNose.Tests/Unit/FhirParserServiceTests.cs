using BlueNose.FhirTesting;
using BlueNose.FhirTesting.Fixtures;
using FluentAssertions;
using Xunit;

namespace BlueNose.Tests.Unit;

/// <summary>
/// Unit tests for FhirParserService
/// </summary>
public class FhirParserServiceTests
{
    [Fact]
    public async Task ParseJsonAsync_R4Patient_ExtractsMetadata()
    {
        // Arrange
        var service = TestServiceFactory.CreateR4ParserService();
        var json = SampleResourcesR4.ValidPatient;

        // Act
        var result = await service.ParseJsonAsync(json);

        // Assert
        result.Success.Should().BeTrue();
        result.ResourceType.Should().Be("Patient");
        result.ResourceId.Should().Be("example-patient-1");
        result.FhirVersion.Should().Be("R4");
        result.Metadata.Should().ContainKey("name");
        result.Metadata.Should().ContainKey("gender");
        result.Metadata.Should().ContainKey("birthDate");
    }

    [Fact]
    public async Task ParseJsonAsync_InvalidJson_ReturnsFalse()
    {
        // Arrange
        var service = TestServiceFactory.CreateR4ParserService();
        var json = SampleResourcesR4.MalformedJson;

        // Act
        var result = await service.ParseJsonAsync(json);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ConvertFormatAsync_JsonToXml_Succeeds()
    {
        // Arrange
        var service = TestServiceFactory.CreateR4ParserService();
        var json = SampleResourcesR4.MinimalPatient;

        // Act
        var xml = await service.ConvertFormatAsync(json, "json", "xml");

        // Assert
        xml.Should().Contain("<Patient");
        xml.Should().Contain("xmlns=\"http://hl7.org/fhir\"");
    }

    [Fact]
    public async Task ConvertFormatAsync_SameFormat_ReturnsOriginal()
    {
        // Arrange
        var service = TestServiceFactory.CreateR4ParserService();
        var json = SampleResourcesR4.MinimalPatient;

        // Act
        var result = await service.ConvertFormatAsync(json, "json", "json");

        // Assert
        result.Should().Be(json);
    }

    [Fact]
    public void CurrentFhirVersion_R4Service_ReturnsR4()
    {
        // Arrange
        var service = TestServiceFactory.CreateR4ParserService();

        // Assert
        service.CurrentFhirVersion.Should().Be("R4");
    }
}
