# 1. IMAGEN DE CONSTRUCCIÓN (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos el archivo de solución y el de proyecto para restaurar dependencias
COPY *.sln .
COPY BusTrackBackEnd.API/*.csproj ./BusTrackBackEnd.API/
RUN dotnet restore

# Copiamos todo el resto de los archivos
COPY . .

# Compilamos y publicamos el proyecto API
WORKDIR "/src/BusTrackBackEnd.API"
RUN dotnet publish -c Release -o /app/out

# 2. IMAGEN DE EJECUCIÓN (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configuración de puertos para Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Comando de inicio (Asegúrate que el nombre del DLL sea correcto)
ENTRYPOINT ["dotnet", "BusTrackBackEnd.API.dll"]
