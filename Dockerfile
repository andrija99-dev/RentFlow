# syntax=docker/dockerfile:1

# ---- Build stage ---------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution-level build configuration first so `dotnet restore` is cached
# independently of source changes (Central Package Management + shared props).
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/Directory.Build.props src/

# Copy only the project files, then restore — keeps the restore layer cacheable.
COPY src/RentFlow.Domain/RentFlow.Domain.csproj src/RentFlow.Domain/
COPY src/RentFlow.Application/RentFlow.Application.csproj src/RentFlow.Application/
COPY src/RentFlow.Infrastructure/RentFlow.Infrastructure.csproj src/RentFlow.Infrastructure/
COPY src/RentFlow.API/RentFlow.API.csproj src/RentFlow.API/
RUN dotnet restore src/RentFlow.API/RentFlow.API.csproj

# Copy the remaining source and publish a framework-dependent build.
COPY src/ src/
RUN dotnet publish src/RentFlow.API/RentFlow.API.csproj \
    -c Release \
    --no-restore \
    -o /app/publish

# ---- Runtime stage -------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Run as the non-root user shipped in the .NET images; give it a writable log dir.
RUN mkdir -p /app/logs && chown -R app:app /app/logs
USER app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RentFlow.API.dll"]
