#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Services/TestManager/VietGeeks.TestPlatform.TestManager.Api/VietGeeks.TestPlatform.TestManager.Api.csproj", "Services/TestManager/VietGeeks.TestPlatform.TestManager.Api/"]
COPY ["Services/TestManager/VietGeeks.TestPlatform.TestManager.Contract/VietGeeks.TestPlatform.TestManager.Contract.csproj", "Services/TestManager/VietGeeks.TestPlatform.TestManager.Contract/"]
COPY ["Services/TestManager/VietGeeks.TestPlatform.TestManager.Infrastructure/VietGeeks.TestPlatform.TestManager.Infrastructure.csproj", "Services/TestManager/VietGeeks.TestPlatform.TestManager.Infrastructure/"]
COPY ["IntegrationBlocks/VietGeeks.TestPlatform.Integration.Contract/VietGeeks.TestPlatform.Integration.Contract.csproj", "IntegrationBlocks/VietGeeks.TestPlatform.Integration.Contract/"]
COPY ["Services/TestManager/VietGeeks.TestPlatform.TestManager.Core/VietGeeks.TestPlatform.TestManager.Core.csproj", "Services/TestManager/VietGeeks.TestPlatform.TestManager.Core/"]
COPY ["BuildingBlocks/VietGeeks.TestPlatform.SharedKernel/VietGeeks.TestPlatform.SharedKernel.csproj", "BuildingBlocks/VietGeeks.TestPlatform.SharedKernel/"]
RUN dotnet restore "Services/TestManager/VietGeeks.TestPlatform.TestManager.Api/VietGeeks.TestPlatform.TestManager.Api.csproj"
COPY . .
WORKDIR "/src/Services/TestManager/VietGeeks.TestPlatform.TestManager.Api"
RUN dotnet build "VietGeeks.TestPlatform.TestManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VietGeeks.TestPlatform.TestManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VietGeeks.TestPlatform.TestManager.Api.dll"]