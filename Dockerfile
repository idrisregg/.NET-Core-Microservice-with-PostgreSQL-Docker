FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS base
WORKDIR /app
EXPOSE 5001

ENV ASPNETCORE_URLS=http://+:5001

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG configuration=Release
WORKDIR /src
COPY ["Wajeb.API.csproj", "./"]
RUN dotnet restore "Wajeb.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Wajeb.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Wajeb.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wajeb.API.dll"]
