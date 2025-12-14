# BlueNose MCP Server Dockerfile
# Multi-stage build for optimized image size

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY BlueNose.sln .
COPY src/BlueNose.Core/BlueNose.Core.csproj src/BlueNose.Core/
COPY src/BlueNose.McpServer/BlueNose.McpServer.csproj src/BlueNose.McpServer/

# Restore dependencies
RUN dotnet restore src/BlueNose.McpServer/BlueNose.McpServer.csproj

# Copy source code
COPY src/BlueNose.Core/ src/BlueNose.Core/
COPY src/BlueNose.McpServer/ src/BlueNose.McpServer/

# Build and publish
WORKDIR /src/src/BlueNose.McpServer
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

# Environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV BLUENOSE_Fhir__Version=R4

# The MCP server uses stdio, so no ports exposed
# It reads from stdin and writes to stdout

ENTRYPOINT ["dotnet", "BlueNose.McpServer.dll"]
