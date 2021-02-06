# Build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY . .

WORKDIR /source/DiscordChatExporter/DiscordChatExporter.Domain
RUN dotnet restore

WORKDIR /source/ExportAPI
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# Run
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ExportAPI.dll"]
