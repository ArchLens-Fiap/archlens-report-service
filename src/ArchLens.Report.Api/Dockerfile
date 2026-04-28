FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Directory.Build.props .

COPY src/SharedKernel/ArchLens.SharedKernel/ArchLens.SharedKernel.csproj src/SharedKernel/ArchLens.SharedKernel/
COPY src/Contracts/ArchLens.Contracts/ArchLens.Contracts.csproj src/Contracts/ArchLens.Contracts/
COPY src/Services/Report/ArchLens.Report.Domain/ArchLens.Report.Domain.csproj src/Services/Report/ArchLens.Report.Domain/
COPY src/Services/Report/ArchLens.Report.Application.Contracts/ArchLens.Report.Application.Contracts.csproj src/Services/Report/ArchLens.Report.Application.Contracts/
COPY src/Services/Report/ArchLens.Report.Application/ArchLens.Report.Application.csproj src/Services/Report/ArchLens.Report.Application/
COPY src/Services/Report/ArchLens.Report.Infrastructure/ArchLens.Report.Infrastructure.csproj src/Services/Report/ArchLens.Report.Infrastructure/
COPY src/Services/Report/ArchLens.Report.Api/ArchLens.Report.Api.csproj src/Services/Report/ArchLens.Report.Api/

RUN dotnet restore src/Services/Report/ArchLens.Report.Api/ArchLens.Report.Api.csproj

COPY src/SharedKernel/ src/SharedKernel/
COPY src/Contracts/ src/Contracts/
COPY src/Services/Report/ src/Services/Report/

RUN dotnet publish src/Services/Report/ArchLens.Report.Api/ArchLens.Report.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
USER $APP_UID
ENTRYPOINT ["dotnet", "ArchLens.Report.Api.dll"]
