# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY AccountService.API/*.csproj AccountService.API/
RUN dotnet restore AccountService.API/AccountService.API.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/AccountService.API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install SQL Server tools for health checks
USER root
RUN apt-get update && apt-get install -y curl

USER app
EXPOSE 5001

ENTRYPOINT ["dotnet", "AccountService.API.dll"]