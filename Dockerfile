# .NET 8 SDK image kullan
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Her iki proje dosyasını da kopyala
COPY ["CityHotelGarageAPI/CityHotelGarageAPI.csproj", "CityHotelGarageAPI/"]
COPY ["CityHotelGarage.Business/CityHotelGarage.Business.csproj", "CityHotelGarage.Business/"]

# NuGet restore
RUN dotnet restore "CityHotelGarageAPI/CityHotelGarageAPI.csproj"

# Tüm kaynak kodları kopyala
COPY . .

# Build
RUN dotnet build "CityHotelGarageAPI/CityHotelGarageAPI.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "CityHotelGarageAPI/CityHotelGarageAPI.csproj" -c Release -o /app/publish

# Runtime - 8.0 tag kullan
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Security packages yükle (curl health check için)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Non-root user oluştur
RUN groupadd -r dotnet && useradd -r -g dotnet dotnet

# Published dosyaları kopyala
COPY --from=publish /app/publish .

# Ownership değiştir
RUN chown -R dotnet:dotnet /app

# Ports
EXPOSE 5010
EXPOSE 5011

# Environment variables
ENV ASPNETCORE_URLS=http://+:5010
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check ekle
HEALTHCHECK --interval=30s --timeout=10s --start-period=15s --retries=3 \
    CMD curl -f http://localhost:5010/health || exit 1

# Non-root user olarak çalıştır
USER dotnet

ENTRYPOINT ["dotnet", "CityHotelGarageAPI.dll"]