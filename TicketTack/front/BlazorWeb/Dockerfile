FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["front/BlazorWeb/BlazorWeb.csproj", "front/BlazorWeb/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "front/BlazorWeb/BlazorWeb.csproj"
COPY . .
WORKDIR "/src/front/BlazorWeb"
RUN dotnet build "BlazorWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorWeb.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorWeb.dll"]