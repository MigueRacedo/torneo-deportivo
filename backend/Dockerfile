# syntax=docker/dockerfile:1

# --- Etapa de build ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Confiar en certificados raíz locales (ej. antivirus con inspección SSL/TLS),
# necesario solo si la red/host intercepta HTTPS hacia nuget.org. La carpeta
# certs/ está vacía por defecto (gitignored salvo .gitkeep); si no hace falta
# ningún certificado custom, este paso no tiene efecto.
COPY certs/ /usr/local/share/ca-certificates/
RUN update-ca-certificates

COPY Directory.Build.props ./
COPY TorneoDeportivo.Domain/TorneoDeportivo.Domain.csproj TorneoDeportivo.Domain/
COPY TorneoDeportivo.Application/TorneoDeportivo.Application.csproj TorneoDeportivo.Application/
COPY TorneoDeportivo.Infrastructure/TorneoDeportivo.Infrastructure.csproj TorneoDeportivo.Infrastructure/
COPY TorneoDeportivo.API/TorneoDeportivo.API.csproj TorneoDeportivo.API/
RUN dotnet restore TorneoDeportivo.API/TorneoDeportivo.API.csproj

COPY TorneoDeportivo.Domain/ TorneoDeportivo.Domain/
COPY TorneoDeportivo.Application/ TorneoDeportivo.Application/
COPY TorneoDeportivo.Infrastructure/ TorneoDeportivo.Infrastructure/
COPY TorneoDeportivo.API/ TorneoDeportivo.API/

RUN dotnet publish TorneoDeportivo.API/TorneoDeportivo.API.csproj -c Release -o /app/publish --no-restore

# --- Etapa de runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "TorneoDeportivo.API.dll"]
