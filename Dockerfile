#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FaceBot.WebApi/FaceBot.WebApi.csproj", "src/FaceBot.WebApi/"]
RUN dotnet restore "./src/FaceBot.WebApi/FaceBot.WebApi.csproj"
COPY . .
WORKDIR "/src/src/FaceBot.WebApi"
RUN dotnet build "./FaceBot.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Install dotnet debug tools
RUN dotnet tool install --tool-path /tools dotnet-trace \
 && dotnet tool install --tool-path /tools dotnet-counters \
 && dotnet tool install --tool-path /tools dotnet-dump \
 && dotnet tool install --tool-path /tools dotnet-gcdump

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FaceBot.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
# Copy dotnet-tools
WORKDIR /tools
COPY --from=publish /tools .

WORKDIR /app
COPY --from=publish /app/publish .
COPY ["src/FaceBot.WebApi/Images", "/app/Images"]


ENTRYPOINT ["dotnet", "FaceBot.WebApi.dll"]
