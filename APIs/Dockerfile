# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["APIs.csproj", "."]
RUN dotnet restore "APIs.csproj"

COPY . .
RUN dotnet publish "APIs.csproj" \
    --configuration Release \
    --output /app/publish

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "APIs.dll"]