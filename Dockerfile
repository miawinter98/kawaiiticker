ARG BASE=8.0

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:$BASE AS base
USER app
WORKDIR /app
EXPOSE 8080
HEALTHCHECK --start-period=5s --start-interval=15s --interval=30s --timeout=30s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG VERSION=0.0.1
WORKDIR /src
COPY ["Kawaiiticker.csproj", "."]
RUN dotnet restore "./Kawaiiticker.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Kawaiiticker.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    -p:Version="${VERSION}"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Kawaiiticker.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    -p:Version="${VERSION}"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY LICENSE .
ENTRYPOINT ["dotnet", "Kawaiiticker.dll"]