FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["global.json", "./"]
COPY ["zeatrix.presentation/zeatrix.presentation.csproj", "zeatrix.presentation/"]
COPY ["Zeatrix.Application/Zeatrix.Application.csproj", "Zeatrix.Application/"]
COPY ["Zeatrix.Infrastructure/Zeatrix.Infrastructure.csproj", "Zeatrix.Infrastructure/"]
COPY ["Zeatrix.Domain/Zeatrix.Domain.csproj", "Zeatrix.Domain/"]
RUN dotnet restore "zeatrix.presentation/zeatrix.presentation.csproj"
COPY . .
WORKDIR "/src/zeatrix.presentation"
RUN dotnet build "./zeatrix.presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./zeatrix.presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "zeatrix.presentation.dll"]
