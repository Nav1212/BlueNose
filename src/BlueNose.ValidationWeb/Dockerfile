# BlueNose Validation Web API Dockerfile
# Multi-stage build for optimized image size

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY BlueNose.sln .
COPY src/BlueNose.Core/BlueNose.Core.csproj src/BlueNose.Core/
COPY src/BlueNose.ValidationWeb/BlueNose.ValidationWeb.csproj src/BlueNose.ValidationWeb/

# Restore dependencies
RUN dotnet restore src/BlueNose.ValidationWeb/BlueNose.ValidationWeb.csproj

# Copy source code
COPY src/BlueNose.Core/ src/BlueNose.Core/
COPY src/BlueNose.ValidationWeb/ src/BlueNose.ValidationWeb/

# Build and publish
WORKDIR /src/src/BlueNose.ValidationWeb
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser

# Copy published app
COPY --from=build /app/publish .

# Change ownership and switch to non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV BLUENOSE_Fhir__Version=R4

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "BlueNose.ValidationWeb.dll"]
