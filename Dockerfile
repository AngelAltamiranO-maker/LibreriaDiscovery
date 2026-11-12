# Fase de compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar primero el archivo de proyecto y restaurar dependencias
COPY ["LibreriaDiscovery.csproj", "./"]
RUN dotnet restore "LibreriaDiscovery.csproj"

# Copiar el resto del código fuente
COPY . .
RUN dotnet publish "LibreriaDiscovery.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Fase de ejecución final
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
ENTRYPOINT ["dotnet", "LibreriaDiscovery.dll"]