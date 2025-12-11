# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["S3Watcher.csproj", "./"]
RUN dotnet restore "S3Watcher.csproj"
COPY . .
RUN dotnet publish "S3Watcher.csproj" -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .


ENTRYPOINT ["dotnet", "S3Watcher.dll"]