# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JobScraperBot/JobScraperBot.csproj", "JobScraperBot/"]
COPY ["JobScraperBot.DAL/JobScraperBot.DAL.csproj", "JobScraperBot.DAL/"]
RUN dotnet restore "./JobScraperBot/JobScraperBot.csproj"
COPY . .
WORKDIR "/src/JobScraperBot"
RUN dotnet build "./JobScraperBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JobScraperBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Use the official .NET runtime for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobScraperBot.dll"]
