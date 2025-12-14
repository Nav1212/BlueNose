using BlueNose.FhirTesting.Fixtures;
using FluentAssertions;
using Hl7.Fhir.Model;
using Xunit;

namespace BlueNose.Tests.Fhir;

/// <summary>
/// Tests for FHIR resource factories and sample data
/// </summary>
public class ResourceFactoryTests
{
    [Fact]
    public void CreatePatient_R4_CreatesValidResource()
    {
        // Act
        var patient = ResourceFactoryR4.CreatePatient(
            id: "test-1",
            familyName: "Smith",
            givenName: "John",
            gender: "male",
            birthDate: "1990-01-15"
        );

        // Assert
        patient.Id.Should().Be("test-1");
        patient.Name.Should().HaveCount(1);
        patient.Name[0].Family.Should().Be("Smith");
        patient.Name[0].Given.Should().Contain("John");
        patient.Gender.Should().Be(AdministrativeGender.Male);
        patient.BirthDate.Should().Be("1990-01-15");
        patient.Active.Should().BeTrue();
    }

    [Fact]
    public void CreateObservation_R4_CreatesValidResource()
    {
        // Act
        var observation = ResourceFactoryR4.CreateObservation(
            id: "obs-1",
            patientReference: "Patient/test-1",
            loincCode: "8867-4",
            display: "Heart rate",
            value: 72,
            unit: "/min"
        );

        // Assert
        observation.Id.Should().Be("obs-1");
        observation.Status.Should().Be(ObservationStatus.Final);
        observation.Subject.Reference.Should().Be("Patient/test-1");
        observation.Code.Coding.Should().HaveCount(1);
        observation.Code.Coding[0].Code.Should().Be("8867-4");
    }

    [Fact]
    public void CreateBundle_R4_ContainsResources()
    {
        // Arrange
        var patient = ResourceFactoryR4.CreatePatient("p1", "Test", "User");
        var observation = ResourceFactoryR4.CreateObservation("o1", "Patient/p1", "8867-4", "Heart rate", 72, "/min");

        // Act
        var bundle = ResourceFactoryR4.CreateBundle(
            "bundle-1",
            Bundle.BundleType.Collection,
            patient,
            observation
        );

        // Assert
        bundle.Id.Should().Be("bundle-1");
        bundle.Type.Should().Be(Bundle.BundleType.Collection);
        bundle.Entry.Should().HaveCount(2);
        bundle.Entry[0].Resource.Should().BeOfType<Patient>();
        bundle.Entry[1].Resource.Should().BeOfType<Observation>();
    }

    [Fact]
    public void ToJson_R4Patient_ReturnsValidJson()
    {
        // Arrange
        var patient = ResourceFactoryR4.CreatePatient("json-test", "Smith", "John");

        // Act
        var json = ResourceFactoryR4.ToJson(patient);

        // Assert
        json.Should().Contain("\"resourceType\": \"Patient\"");
        json.Should().Contain("\"id\": \"json-test\"");
        json.Should().Contain("\"family\": \"Smith\"");
    }

    [Fact]
    public void SampleResourcesR4_ValidPatient_IsValidJson()
    {
        // Assert
        var json = SampleResourcesR4.ValidPatient;
        json.Should().Contain("\"resourceType\": \"Patient\"");
        json.Should().Contain("\"id\": \"example-patient-1\"");
    }
}
