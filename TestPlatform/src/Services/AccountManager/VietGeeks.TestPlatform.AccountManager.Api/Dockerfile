#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Api/VietGeeks.TestPlatform.AccountManager.Api.csproj", "Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Api/"]
COPY ["Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Infrastructure/VietGeeks.TestPlatform.AccountManager.Infrastructure.csproj", "Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Infrastructure/"]
COPY ["Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Contract/VietGeeks.TestPlatform.AccountManager.Contract.csproj", "Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Contract/"]
COPY ["Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Core/VietGeeks.TestPlatform.AccountManager.Core.csproj", "Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Core/"]
COPY ["IntegrationBlocks/VietGeeks.TestPlatform.Integration.Contract/VietGeeks.TestPlatform.Integration.Contract.csproj", "IntegrationBlocks/VietGeeks.TestPlatform.Integration.Contract/"]
RUN dotnet restore "Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Api/VietGeeks.TestPlatform.AccountManager.Api.csproj"
COPY . .
WORKDIR "/src/Services/AccountManager/VietGeeks.TestPlatform.AccountManager.Api"
RUN dotnet build "VietGeeks.TestPlatform.AccountManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VietGeeks.TestPlatform.AccountManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VietGeeks.TestPlatform.AccountManager.Api.dll"]