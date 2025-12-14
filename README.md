# BlueNose - FHIR MCP Platform

A .NET 8 platform providing FHIR (Fast Healthcare Interoperability Resources) validation and data processing through Model Context Protocol (MCP) integration.

## Solution Structure

```
BlueNose/
├── BlueNose.sln                    # Solution file
├── Directory.Build.props           # Shared build properties
├── Directory.Packages.props        # Centralized NuGet package versions
│
├── src/
│   ├── BlueNose.Core/              # Shared FHIR logic and API models
│   ├── BlueNose.McpServer/         # MCP Server (stdio transport)
│   ├── BlueNose.ValidationWeb/     # Web API with SSE MCP support
│   └── BlueNose.FhirTesting/       # FHIR test fixtures and utilities
│
└── tests/
    └── BlueNose.Tests/             # xUnit test suite
```

## Projects

### BlueNose.Core
Shared class library containing:
- FHIR R4 and R5 abstractions with config-based version switching
- Validation services using Firely SDK
- Common models and interfaces

### BlueNose.McpServer
Console application providing MCP server functionality:
- Stdio transport for CLI integration (e.g., Claude Desktop)
- FHIR validation and resource manipulation tools

### BlueNose.ValidationWeb
ASP.NET Core Web API providing:
- REST endpoints for FHIR validation
- SSE-based MCP transport via `/mcp` endpoint
- Swagger/OpenAPI documentation

### BlueNose.FhirTesting
Testing utilities library:
- Sample FHIR resources (Patient, Observation, etc.)
- Conformance testing helpers
- Mock FHIR server utilities

### BlueNose.Tests
xUnit test project with:
- Unit tests for all components
- Integration tests
- FHIR-specific conformance tests

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Docker (optional, for containerized deployment)

### Build
```bash
dotnet restore
dotnet build
```

### Run MCP Server (stdio)
```bash
dotnet run --project src/BlueNose.McpServer
```

### Run Validation Web API
```bash
dotnet run --project src/BlueNose.ValidationWeb
```

### Run Tests
```bash
dotnet test
```

## Docker Deployment

### Build Images
```bash
docker build -f src/BlueNose.McpServer/Dockerfile -t bluenose-mcp-server .
docker build -f src/BlueNose.ValidationWeb/Dockerfile -t bluenose-validation-web .
```

### Run Containers
```bash
# MCP Server (for use with stdio-based clients)
docker run -it bluenose-mcp-server

# Validation Web API
docker run -p 5000:8080 bluenose-validation-web
```

## Configuration

FHIR version can be configured via `appsettings.json`:

```json
{
  "Fhir": {
    "Version": "R4",  // or "R5"
    "ValidateOnParse": true
  }
}
```

## License
MIT
