ARG BUILD_IMAGE=mcr.microsoft.com/dotnet/sdk:8.0-noble

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM $BUILD_IMAGE AS prepare-restore-files
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset
WORKDIR /src
COPY . .
RUN dotnet subset restore VietGeeksTestPlatform.sln --root-directory /src --output /restore_subset

FROM $BUILD_IMAGE AS restore
WORKDIR /src
COPY --from=prepare-restore-files /restore_subset .
RUN dotnet restore

FROM restore AS build-backoffice
ARG BUILD_CONFIGURATION=Release
COPY . .
WORKDIR "/src/VietGeeks.TestPlatform.BackOffice"
RUN dotnet build "./VietGeeks.TestPlatform.BackOffice.csproj" -c $BUILD_CONFIGURATION --no-restore

FROM build-backoffice AS publish-backoffice
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VietGeeks.TestPlatform.BackOffice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-build --no-restore

FROM base AS final-backoffice
WORKDIR /app
COPY --from=publish-backoffice /app/publish .
ENTRYPOINT ["dotnet", "VietGeeks.TestPlatform.BackOffice.dll"]

FROM restore AS build-frontoffice
ARG BUILD_CONFIGURATION=Release
COPY . .
WORKDIR "/src/VietGeeks.TestPlatform.FrontOffice"
RUN dotnet build "./VietGeeks.TestPlatform.FrontOffice.csproj" -c $BUILD_CONFIGURATION --no-restore

FROM build-frontoffice AS publish-frontoffice
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VietGeeks.TestPlatform.FrontOffice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-build --no-restore

FROM base AS final-frontoffice
WORKDIR /app
COPY --from=publish-frontoffice /app/publish .
ENTRYPOINT ["dotnet", "VietGeeks.TestPlatform.FrontOffice.dll"]