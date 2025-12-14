using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace BlueNose.FhirTesting.Fixtures;

/// <summary>
/// Factory for creating sample FHIR resources programmatically (R4)
/// </summary>
public static class ResourceFactoryR4
{
    /// <summary>
    /// Creates a new Patient resource with the specified data
    /// </summary>
    public static Patient CreatePatient(
        string id,
        string familyName,
        string givenName,
        string? gender = "male",
        string? birthDate = null)
    {
        var patient = new Patient
        {
            Id = id,
            Active = true,
            Gender = gender?.ToLower() switch
            {
                "male" => AdministrativeGender.Male,
                "female" => AdministrativeGender.Female,
                "other" => AdministrativeGender.Other,
                _ => AdministrativeGender.Unknown
            }
        };

        patient.Name.Add(new HumanName
        {
            Use = HumanName.NameUse.Official,
            Family = familyName,
            Given = new[] { givenName }
        });

        if (!string.IsNullOrEmpty(birthDate))
        {
            patient.BirthDate = birthDate;
        }

        return patient;
    }

    /// <summary>
    /// Creates a new Observation resource
    /// </summary>
    public static Observation CreateObservation(
        string id,
        string patientReference,
        string loincCode,
        string display,
        decimal value,
        string unit)
    {
        var observation = new Observation
        {
            Id = id,
            Status = ObservationStatus.Final,
            Subject = new ResourceReference
            {
                Reference = patientReference
            },
            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new()
                    {
                        System = "http://loinc.org",
                        Code = loincCode,
                        Display = display
                    }
                },
                Text = display
            },
            Value = new Quantity
            {
                Value = value,
                Unit = unit,
                System = "http://unitsofmeasure.org",
                Code = unit
            },
            Effective = new FhirDateTime(DateTimeOffset.UtcNow)
        };

        return observation;
    }

    /// <summary>
    /// Creates a Bundle containing the specified resources
    /// </summary>
    public static Bundle CreateBundle(
        string id,
        Bundle.BundleType bundleType,
        params Resource[] resources)
    {
        var bundle = new Bundle
        {
            Id = id,
            Type = bundleType
        };

        foreach (var resource in resources)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                Resource = resource,
                FullUrl = $"urn:uuid:{Guid.NewGuid()}"
            });
        }

        return bundle;
    }

    /// <summary>
    /// Serializes a FHIR resource to JSON
    /// </summary>
    public static string ToJson(Resource resource, bool pretty = true)
    {
        var serializer = new FhirJsonSerializer(
            new SerializerSettings { Pretty = pretty });
        return serializer.SerializeToString(resource);
    }
}
