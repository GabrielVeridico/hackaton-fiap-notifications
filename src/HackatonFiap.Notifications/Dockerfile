FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/HackatonFiap.Notifications/HackatonFiap.Notifications.csproj", "src/HackatonFiap.Notifications/"]
RUN dotnet restore "src/HackatonFiap.Notifications/HackatonFiap.Notifications.csproj"
COPY src/HackatonFiap.Notifications/ src/HackatonFiap.Notifications/
RUN dotnet publish "src/HackatonFiap.Notifications/HackatonFiap.Notifications.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot
