# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY TrendyolClone/*.csproj TrendyolClone/
RUN dotnet restore "TrendyolClone/TrendyolClone.csproj"

# Copy everything and build
COPY TrendyolClone/ TrendyolClone/
WORKDIR /src/TrendyolClone
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TrendyolClone.dll"]

