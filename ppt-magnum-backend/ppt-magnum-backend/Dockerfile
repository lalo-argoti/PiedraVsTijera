# Usa una imagen base de .NET SDK 8.0 para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo .csproj y restaura las dependencias
COPY ["ppt-magnum-backend.csproj", "./"]
RUN dotnet restore "ppt-magnum-backend.csproj"

# Copia el resto del código fuente y construye la aplicación
COPY . .
RUN dotnet build "ppt-magnum-backend.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "ppt-magnum-backend.csproj" -c Release -o /app/publish

# Usa una imagen base de .NET Runtime 8.0 para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expone el puerto en el que la aplicación escucha
EXPOSE 8000

# Define el comando de entrada
ENTRYPOINT ["dotnet", "ppt-magnum-backend.dll"]
