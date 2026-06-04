# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/Directory.Build.props src/

COPY src/RentFlow.Domain/RentFlow.Domain.csproj src/RentFlow.Domain/
COPY src/RentFlow.Application/RentFlow.Application.csproj src/RentFlow.Application/
COPY src/RentFlow.Infrastructure/RentFlow.Infrastructure.csproj src/RentFlow.Infrastructure/
COPY src/RentFlow.API/RentFlow.API.csproj src/RentFlow.API/
RUN dotnet restore src/RentFlow.API/RentFlow.API.csproj

COPY src/ src/
RUN dotnet publish src/RentFlow.API/RentFlow.API.csproj \
    -c Release \
    --no-restore \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/logs && chown -R app:app /app/logs
USER app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RentFlow.API.dll"]
