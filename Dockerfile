# Multi-stage build for .NET 8 API with JWT
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY *.sln ./

# Copy project files with correct paths
COPY CityHotelGarageAPI/CityHotelGarageAPI.csproj ./CityHotelGarageAPI/
COPY CityHotelGarage.Business/CityHotelGarage.Business.csproj ./CityHotelGarage.Business/

# Restore NuGet packages
RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/CityHotelGarageAPI
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Set environment for JWT and API
ENV ASPNETCORE_URLS=http://+:5010
ENV ASPNETCORE_ENVIRONMENT=Development

# Health check for container monitoring
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5010/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "CityHotelGarageAPI.dll"]