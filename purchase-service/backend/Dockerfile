FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PurchaseService.API/PurchaseService.API.csproj", "PurchaseService.API/"]
RUN dotnet restore "PurchaseService.API/PurchaseService.API.csproj"
COPY . .
WORKDIR "/src/PurchaseService.API"
RUN dotnet build "PurchaseService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PurchaseService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PurchaseService.API.dll"]