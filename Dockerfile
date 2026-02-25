# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["TrendyolClone/TrendyolClone.csproj", "TrendyolClone/"]
RUN dotnet restore "TrendyolClone/TrendyolClone.csproj"

# Copy everything else and build
COPY TrendyolClone/ TrendyolClone/
WORKDIR /src/TrendyolClone
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

# Expose port (Render uses PORT environment variable)
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TrendyolClone.dll"]
