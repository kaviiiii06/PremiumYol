# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["TrendyolClone.csproj", "./"]
RUN dotnet restore "TrendyolClone.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "TrendyolClone.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TrendyolClone.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create directories for data and logs
RUN mkdir -p /app/data /app/Logs

# Copy published app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "TrendyolClone.dll"]
