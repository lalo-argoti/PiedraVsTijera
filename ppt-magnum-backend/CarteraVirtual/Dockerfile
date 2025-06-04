# Usa imagen oficial de .NET SDK para compilación (fase 1)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copia archivos de proyecto y restaura dependencias usando caché
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código fuente
COPY . ./

# Publica la aplicación (genera binarios listos para producción)
RUN dotnet publish -c Release -o out

# Imagen final con .NET Runtime (fase 2)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8000

ENTRYPOINT ["dotnet", "CarteraVirtual.dll"]
