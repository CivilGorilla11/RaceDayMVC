# --- Stage 1: Build & Publish ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["RaceDay/RaceDay.csproj", "RaceDay/"]
RUN dotnet restore "RaceDay/RaceDay.csproj"

# Copy the rest of the source code and publish
COPY . .
WORKDIR "/src/RaceDay"
RUN dotnet publish "RaceDay.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Stage 2: Runtime Image ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published artifacts from build stage
COPY --from=build /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "RaceDay.dll"]
