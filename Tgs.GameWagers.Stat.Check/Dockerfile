#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameWagers.Stat.Check/Tgs.GameWagers.Stat.Check.csproj", "Tgs.GameWagers.Stat.Check/"]
COPY ["Tgs.GameWagers.Stat.Check.Common/Tgs.GameWagers.Stat.Check.Common.csproj", "Tgs.GameWagers.Stat.Check.Common/"]
COPY ["Tgs.GameWagers.Stat.Check.Core/Tgs.GameWagers.Stat.Check.Core.csproj", "Tgs.GameWagers.Stat.Check.Core/"]
COPY ["Tgs.GameWagers.Stat.Check.Repositories/Tgs.GameWagers.Stat.Check.Repositories.csproj", "Tgs.GameWagers.Stat.Check.Repositories/"]
RUN dotnet restore "./Tgs.GameWagers.Stat.Check/./Tgs.GameWagers.Stat.Check.csproj"
COPY . .
WORKDIR "/src/Tgs.GameWagers.Stat.Check"
RUN dotnet build "./Tgs.GameWagers.Stat.Check.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameWagers.Stat.Check.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameWagers.Stat.Check.dll"]