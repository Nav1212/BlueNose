using System.Net.Http.Json;
using BlueNose.Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BlueNose.Tests.Integration;

/// <summary>
/// Integration tests for ValidationWeb API endpoints
/// </summary>
public class ValidationApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ValidationApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task GetVersion_ReturnsFhirVersion()
    {
        // Act
        var response = await _client.GetAsync("/api/validation/version");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        (content.Contains("R4") || content.Contains("R5")).Should().BeTrue();
        content.Should().Contain("supportedVersions");
    }

    [Fact]
    public async Task PostValidate_ValidPatient_ReturnsSuccess()
    {
        // Arrange
        var request = new ValidationRequest
        {
            ResourceContent = """
                {
                    "resourceType": "Patient",
                    "id": "test-patient",
                    "name": [{"family": "Test", "given": ["User"]}]
                }
                """,
            ContentType = "application/fhir+json"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/validation/validate", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ValidationResult>();
        result.Should().NotBeNull();
        result!.IsValid.Should().BeTrue();
        result.ResourceType.Should().Be("Patient");
    }

    [Fact]
    public async Task PostValidateJson_ValidPatient_ReturnsSuccess()
    {
        // Arrange
        var json = """
            {
                "resourceType": "Patient",
                "id": "test-patient-2"
            }
            """;
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/fhir+json");

        // Act
        var response = await _client.PostAsync("/api/validation/validate/json", content);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PostValidate_EmptyContent_ReturnsBadRequest()
    {
        // Arrange
        var request = new ValidationRequest
        {
            ResourceContent = "",
            ContentType = "application/fhir+json"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/validation/validate", request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
