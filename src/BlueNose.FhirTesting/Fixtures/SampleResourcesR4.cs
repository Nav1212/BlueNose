namespace BlueNose.FhirTesting.Fixtures;

/// <summary>
/// Provides sample FHIR R4 resources for testing
/// </summary>
public static class SampleResourcesR4
{
    /// <summary>
    /// Valid Patient resource with complete data
    /// </summary>
    public static string ValidPatient => """
        {
            "resourceType": "Patient",
            "id": "example-patient-1",
            "meta": {
                "versionId": "1",
                "lastUpdated": "2024-01-15T10:30:00Z"
            },
            "identifier": [
                {
                    "system": "http://hospital.example.org/patients",
                    "value": "12345"
                }
            ],
            "active": true,
            "name": [
                {
                    "use": "official",
                    "family": "Smith",
                    "given": ["John", "William"]
                }
            ],
            "telecom": [
                {
                    "system": "phone",
                    "value": "+1-555-555-1234",
                    "use": "home"
                },
                {
                    "system": "email",
                    "value": "john.smith@example.com"
                }
            ],
            "gender": "male",
            "birthDate": "1990-05-15",
            "address": [
                {
                    "use": "home",
                    "line": ["123 Main Street", "Apt 4B"],
                    "city": "Boston",
                    "state": "MA",
                    "postalCode": "02101",
                    "country": "USA"
                }
            ]
        }
        """;

    /// <summary>
    /// Minimal valid Patient resource
    /// </summary>
    public static string MinimalPatient => """
        {
            "resourceType": "Patient",
            "id": "minimal-patient"
        }
        """;

    /// <summary>
    /// Valid Observation resource (vital signs)
    /// </summary>
    public static string ValidObservation => """
        {
            "resourceType": "Observation",
            "id": "blood-pressure-1",
            "meta": {
                "versionId": "1",
                "lastUpdated": "2024-01-15T14:30:00Z"
            },
            "status": "final",
            "category": [
                {
                    "coding": [
                        {
                            "system": "http://terminology.hl7.org/CodeSystem/observation-category",
                            "code": "vital-signs",
                            "display": "Vital Signs"
                        }
                    ]
                }
            ],
            "code": {
                "coding": [
                    {
                        "system": "http://loinc.org",
                        "code": "85354-9",
                        "display": "Blood pressure panel"
                    }
                ],
                "text": "Blood Pressure"
            },
            "subject": {
                "reference": "Patient/example-patient-1",
                "display": "John Smith"
            },
            "effectiveDateTime": "2024-01-15T14:00:00Z",
            "component": [
                {
                    "code": {
                        "coding": [
                            {
                                "system": "http://loinc.org",
                                "code": "8480-6",
                                "display": "Systolic blood pressure"
                            }
                        ]
                    },
                    "valueQuantity": {
                        "value": 120,
                        "unit": "mmHg",
                        "system": "http://unitsofmeasure.org",
                        "code": "mm[Hg]"
                    }
                },
                {
                    "code": {
                        "coding": [
                            {
                                "system": "http://loinc.org",
                                "code": "8462-4",
                                "display": "Diastolic blood pressure"
                            }
                        ]
                    },
                    "valueQuantity": {
                        "value": 80,
                        "unit": "mmHg",
                        "system": "http://unitsofmeasure.org",
                        "code": "mm[Hg]"
                    }
                }
            ]
        }
        """;

    /// <summary>
    /// Valid Condition resource
    /// </summary>
    public static string ValidCondition => """
        {
            "resourceType": "Condition",
            "id": "condition-1",
            "clinicalStatus": {
                "coding": [
                    {
                        "system": "http://terminology.hl7.org/CodeSystem/condition-clinical",
                        "code": "active"
                    }
                ]
            },
            "verificationStatus": {
                "coding": [
                    {
                        "system": "http://terminology.hl7.org/CodeSystem/condition-ver-status",
                        "code": "confirmed"
                    }
                ]
            },
            "category": [
                {
                    "coding": [
                        {
                            "system": "http://terminology.hl7.org/CodeSystem/condition-category",
                            "code": "encounter-diagnosis",
                            "display": "Encounter Diagnosis"
                        }
                    ]
                }
            ],
            "code": {
                "coding": [
                    {
                        "system": "http://snomed.info/sct",
                        "code": "73211009",
                        "display": "Diabetes mellitus"
                    }
                ],
                "text": "Diabetes mellitus"
            },
            "subject": {
                "reference": "Patient/example-patient-1"
            },
            "onsetDateTime": "2020-06-15"
        }
        """;

    /// <summary>
    /// Valid Bundle resource with multiple entries
    /// </summary>
    public static string ValidBundle => """
        {
            "resourceType": "Bundle",
            "id": "bundle-example",
            "type": "collection",
            "entry": [
                {
                    "resource": {
                        "resourceType": "Patient",
                        "id": "patient-in-bundle",
                        "name": [
                            {
                                "family": "Doe",
                                "given": ["Jane"]
                            }
                        ]
                    }
                }
            ]
        }
        """;

    /// <summary>
    /// Invalid Patient resource (missing resourceType)
    /// </summary>
    public static string InvalidPatientMissingType => """
        {
            "id": "invalid-patient",
            "name": [
                {
                    "family": "Invalid"
                }
            ]
        }
        """;

    /// <summary>
    /// Invalid Patient resource (wrong data type)
    /// </summary>
    public static string InvalidPatientWrongType => """
        {
            "resourceType": "Patient",
            "id": "invalid-patient",
            "birthDate": "not-a-date"
        }
        """;

    /// <summary>
    /// Malformed JSON
    /// </summary>
    public static string MalformedJson => """
        {
            "resourceType": "Patient",
            "id": "malformed"
            "name": [
        }
        """;
}
