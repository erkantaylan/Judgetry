FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/Directory.Packages.props","/"]
COPY ["Source/Judgetry.Web/Judgetry.Web.csproj", "Source/Judgetry.Web/"]
COPY ["Source/Judgetry.Core/Judgetry.Core.csproj", "Source/Judgetry.Core/"]
RUN dotnet restore "Source/Judgetry.Web/Judgetry.Web.csproj"
COPY . .
WORKDIR "/src/Source/Judgetry.Web"
RUN dotnet build "Judgetry.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Judgetry.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Judgetry.Web.dll"]
