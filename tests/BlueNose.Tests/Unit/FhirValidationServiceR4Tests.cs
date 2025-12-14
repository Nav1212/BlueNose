using BlueNose.FhirTesting;
using BlueNose.FhirTesting.Fixtures;
using FluentAssertions;
using Xunit;

namespace BlueNose.Tests.Unit;

/// <summary>
/// Unit tests for FhirValidationService with R4 resources
/// </summary>
public class FhirValidationServiceR4Tests
{
    private readonly Core.Services.IFhirValidationService _service;

    public FhirValidationServiceR4Tests()
    {
        _service = TestServiceFactory.CreateR4ValidationService();
    }

    [Fact]
    public async Task ValidateJsonAsync_ValidPatient_ReturnsSuccess()
    {
        // Arrange
        var json = SampleResourcesR4.ValidPatient;

        // Act
        var result = await _service.ValidateJsonAsync(json);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ResourceType.Should().Be("Patient");
        result.FhirVersion.Should().Be("R4");
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateJsonAsync_MinimalPatient_ReturnsSuccess()
    {
        // Arrange
        var json = SampleResourcesR4.MinimalPatient;

        // Act
        var result = await _service.ValidateJsonAsync(json);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ResourceType.Should().Be("Patient");
    }

    [Fact]
    public async Task ValidateJsonAsync_ValidObservation_ReturnsSuccess()
    {
        // Arrange
        var json = SampleResourcesR4.ValidObservation;

        // Act
        var result = await _service.ValidateJsonAsync(json);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ResourceType.Should().Be("Observation");
    }

    [Fact]
    public async Task ValidateJsonAsync_ValidCondition_ReturnsSuccess()
    {
        // Arrange
        var json = SampleResourcesR4.ValidCondition;

        // Act
        var result = await _service.ValidateJsonAsync(json);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ResourceType.Should().Be("Condition");
    }

    [Fact]
    public async Task ValidateJsonAsync_ValidBundle_ReturnsSuccess()
    {
        // Arrange
        var json = SampleResourcesR4.ValidBundle;

        // Act
        var result = await _service.ValidateJsonAsync(json);

        // Assert
        result.IsValid.Should().BeTrue();
        result.ResourceType.Should().Be("Bundle");
    }

    [Fact]
    public async Task ValidateJsonAsync_MalformedJson_ReturnsError()
    {
        // Arrange
        var json = SampleResourcesR4.MalformedJson;

        // Act
        var result = await _service.ValidateJsonAsync(json);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ValidateJsonAsync_EmptyString_ReturnsError()
    {
        // Arrange
        var json = "";

        // Act
        var result = await _service.ValidateJsonAsync(json);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CurrentFhirVersion_ReturnsR4()
    {
        // Assert
        _service.CurrentFhirVersion.Should().Be("R4");
    }
}
