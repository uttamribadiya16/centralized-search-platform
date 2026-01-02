# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY TransportService.API/TransportService.API.csproj ./TransportService.API/
RUN dotnet restore TransportService.API/TransportService.API.csproj

# Copy the source code and build the application
COPY . .
RUN dotnet build TransportService.API/TransportService.API.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish TransportService.API/TransportService.API.csproj -c Release -o /app/publish

# Use the official .NET 8.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 5005

# Set the entry point
ENTRYPOINT ["dotnet", "TransportService.API.dll"]