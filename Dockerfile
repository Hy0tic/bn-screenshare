﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["bnAPI/bnAPI.csproj", "bnAPI/"]
RUN dotnet restore "bnAPI/bnAPI.csproj"
COPY . .
WORKDIR "/src/bnAPI"
RUN dotnet build "bnAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bnAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bnAPI.dll"]
