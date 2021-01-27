# Build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

COPY . ./
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# Run
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ExportAPI.dll"]
