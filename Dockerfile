FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY archlens-contracts/Directory.Build.props ./archlens-contracts/
COPY archlens-contracts/src/ArchLens.SharedKernel/*.csproj ./archlens-contracts/src/ArchLens.SharedKernel/
COPY archlens-contracts/src/ArchLens.Contracts/*.csproj ./archlens-contracts/src/ArchLens.Contracts/

COPY archlens-report-service/*.sln ./archlens-report-service/
COPY archlens-report-service/Directory.Build.props ./archlens-report-service/
COPY archlens-report-service/src/ArchLens.Report.Api/*.csproj ./archlens-report-service/src/ArchLens.Report.Api/
COPY archlens-report-service/src/ArchLens.Report.Application/*.csproj ./archlens-report-service/src/ArchLens.Report.Application/
COPY archlens-report-service/src/ArchLens.Report.Application.Contracts/*.csproj ./archlens-report-service/src/ArchLens.Report.Application.Contracts/
COPY archlens-report-service/src/ArchLens.Report.Domain/*.csproj ./archlens-report-service/src/ArchLens.Report.Domain/
COPY archlens-report-service/src/ArchLens.Report.Infrastructure/*.csproj ./archlens-report-service/src/ArchLens.Report.Infrastructure/

WORKDIR /src/archlens-report-service
RUN dotnet restore src/ArchLens.Report.Api/ArchLens.Report.Api.csproj

WORKDIR /src
COPY archlens-contracts/ ./archlens-contracts/
COPY archlens-report-service/ ./archlens-report-service/

WORKDIR /src/archlens-report-service
RUN dotnet publish src/ArchLens.Report.Api -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
LABEL org.opencontainers.image.source="https://github.com/ArchLens-Fiap/archlens-report-service"
LABEL org.opencontainers.image.title="ArchLens Report Service"
LABEL org.opencontainers.image.version="1.0.0"
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=15s --timeout=5s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ArchLens.Report.Api.dll"]
