FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["WaterApp.API/WaterApp.API.csproj", "WaterApp.API/"]
COPY ["WaterApp.Application/WaterApp.Application.csproj", "WaterApp.Application/"]
COPY ["WaterApp.Infrastructure/WaterApp.Infrastructure.csproj", "WaterApp.Infrastructure/"]
COPY ["WaterApp.Domain/WaterApp.Domain.csproj", "WaterApp.Domain/"]
RUN dotnet restore "WaterApp.API/WaterApp.API.csproj"

COPY . .
WORKDIR /src/WaterApp.API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WaterApp.API.dll"]
