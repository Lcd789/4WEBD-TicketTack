FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/EventService/EventService.csproj", "src/EventService/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/EventService/EventService.csproj"
COPY . .
WORKDIR "/src/src/EventService"
RUN dotnet build "EventService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventService.dll"]