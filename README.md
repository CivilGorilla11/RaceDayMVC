FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src
COPY RaceDay.sln .
COPY RaceDay/ RaceDay/
RUN dotnet restore RaceDay/RaceDay.csproj
RUN dotnet build RaceDay/RaceDay.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=builder /app/build .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_CONTENTROOT=/app
ENTRYPOINT ["dotnet", "RaceDay.dll"]
