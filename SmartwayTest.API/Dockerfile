#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SmartwayTest.API/SmartwayTest.API.csproj", "SmartwayTest.API/"]
COPY ["SmartwayTest.DAL/SmartwayTest.DAL.csproj", "SmartwayTest.DAL/"]
COPY ["SmartwayTest.Core/SmartwayTest.Core.csproj", "SmartwayTest.Core/"]
RUN dotnet restore "./SmartwayTest.API/./SmartwayTest.API.csproj"
COPY . .
WORKDIR "/src/SmartwayTest.API"
RUN dotnet build "./SmartwayTest.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SmartwayTest.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartwayTest.API.dll"]