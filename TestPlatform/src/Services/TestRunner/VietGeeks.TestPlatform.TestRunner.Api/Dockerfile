#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["VietGeeks.TestPlatform.TestRunner.Api/VietGeeks.TestPlatform.TestRunner.Api.csproj", "VietGeeks.TestPlatform.TestRunner.Api/"]
COPY ["VietGeeks.TestPlatform.TestRunner.Actors.Interfaces/VietGeeks.TestPlatform.TestRunner.Actors.Interfaces.csproj", "VietGeeks.TestPlatform.TestRunner.Actors.Interfaces/"]
RUN dotnet restore "VietGeeks.TestPlatform.TestRunner.Api/VietGeeks.TestPlatform.TestRunner.Api.csproj"
COPY . .
WORKDIR "/src/VietGeeks.TestPlatform.TestRunner.Api"
RUN dotnet build "VietGeeks.TestPlatform.TestRunner.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VietGeeks.TestPlatform.TestRunner.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VietGeeks.TestPlatform.TestRunner.Api.dll"]