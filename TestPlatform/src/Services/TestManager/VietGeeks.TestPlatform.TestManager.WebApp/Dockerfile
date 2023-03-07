#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:16.13.0 as node-build
WORKDIR /client/src
COPY ["Services/TestManager/VietGeeks.TestPlatform.TestManager.WebApp/ClientApp/package.json", "."]
COPY ["Services/TestManager/VietGeeks.TestPlatform.TestManager.WebApp/ClientApp/package-lock.json", "."]
RUN npm install -g npm@9.2.0
RUN npm install --force
COPY ["Services/TestManager/VietGeeks.TestPlatform.TestManager.WebApp/ClientApp/", "."]
RUN npm run build:test-manager

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["global.json", "global.json"]
COPY ["NuGet.config", "."]
COPY ["Services/TestManager/VietGeeks.TestPlatform.TestManager.WebApp/VietGeeks.TestPlatform.TestManager.WebApp.csproj", "Services/TestManager/VietGeeks.TestPlatform.TestManager.WebApp/"]
RUN dotnet restore "Services/TestManager/VietGeeks.TestPlatform.TestManager.WebApp/VietGeeks.TestPlatform.TestManager.WebApp.csproj"
COPY --from=node-build /client/src/dist/apps/test-manager /src/Services/TestManager/VietGeeks.TestPlatform.TestManager.WebApp/wwwroot/
COPY . .
WORKDIR "/src/Services/TestManager/VietGeeks.TestPlatform.TestManager.WebApp"
RUN dotnet build "VietGeeks.TestPlatform.TestManager.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VietGeeks.TestPlatform.TestManager.WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VietGeeks.TestPlatform.TestManager.WebApp.dll"]