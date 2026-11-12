# Fase de compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["LibreriaDiscovery.csproj", "."]
RUN dotnet restore "LibreriaDiscovery.csproj"
COPY . .
RUN dotnet publish "LibreriaDiscovery.csproj" -c Release -o /app/publish

# Fase de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
ENTRYPOINT ["dotnet", "LibreriaDiscovery.dll"]