# ========= BUILD =========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore cacheable
COPY ppt-magnum-backend.csproj .
RUN dotnet restore

# Código fuente
COPY . .
RUN dotnet publish -c Release -o /app/out --no-restore

# ========= RUNTIME =========
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 8000
ENTRYPOINT ["dotnet", "ppt-magnum-backend.dll"]
