# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore
COPY TrendyolClone/TrendyolClone.csproj TrendyolClone/
RUN dotnet restore "TrendyolClone/TrendyolClone.csproj"

# Copy source and publish
COPY TrendyolClone/ TrendyolClone/
WORKDIR /src/TrendyolClone
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TrendyolClone.dll"]

