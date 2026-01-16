FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:${PORT} \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_GC_SERVER=true \
    DOTNET_GC_CONCURRENT=true

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ["Wajeb.API.csproj", "./"]
RUN dotnet restore "Wajeb.API.csproj"
COPY . .
RUN dotnet build "Wajeb.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wajeb.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wajeb.API.dll"]
