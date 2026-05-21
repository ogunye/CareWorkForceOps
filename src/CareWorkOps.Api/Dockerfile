FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/CareWorkOps.Api/CareWorkOps.Api.csproj", "src/CareWorkOps.Api/"]
COPY ["src/CareWorkOps.Application/CareWorkOps.Application.csproj", "src/CareWorkOps.Application/"]
COPY ["src/CareWorkOps.Domain/CareWorkOps.Domain.csproj", "src/CareWorkOps.Domain/"]
COPY ["src/CareWorkOps.Infrastructure/CareWorkOps.Infrastructure.csproj", "src/CareWorkOps.Infrastructure/"]
COPY ["src/CareWorkOps.Persistence/CareWorkOps.Persistence.csproj", "src/CareWorkOps.Persistence/"]

RUN dotnet restore "./src/CareWorkOps.Api/CareWorkOps.Api.csproj"

COPY . .

WORKDIR "/src/src/CareWorkOps.Api"

RUN dotnet build "./CareWorkOps.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CareWorkOps.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CareWorkOps.Api.dll"]