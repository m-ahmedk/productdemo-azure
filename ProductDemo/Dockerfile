# ---- Base runtime image ----
# This is the lightweight runtime environment where the app will finally run.
# It only contains the ASP.NET Core runtime (no SDK/compilers).
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080   # Expose port 8080 inside container (mapped in docker-compose.yml)

# ---- Build image ----
# This stage contains the full .NET SDK so it can restore/build/publish the project.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only csproj + solution files first.
# This allows Docker to cache "dotnet restore" so you don't re-download dependencies every time,
# unless the project files change.
COPY ProductDemo/ProductDemo.csproj ProductDemo/
COPY ProductDemo.Tests/ProductDemo.Tests.csproj ProductDemo.Tests/
COPY ProductDemo.IntegrationTests/ProductDemo.IntegrationTests.csproj ProductDemo.IntegrationTests/
COPY *.sln ./

# Restore NuGet dependencies.
RUN dotnet restore

# Copy the rest of the source code into the container.
# At this point, your app + tests + all other files are included.
COPY . .

# Build the application in Release mode (faster, optimized output).
WORKDIR /src/ProductDemo
RUN dotnet build -c Release -o /app/build

# ---- Publish image ----
# Publishes the app into a self-contained folder (/app/publish).
# This trims out unnecessary build files and prepares the app for deployment.
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---- Final runtime image ----
# Start fresh from the small runtime image (no SDK, so it's smaller and more secure).
FROM base AS final
WORKDIR /app

# Copy the published app from the "publish" stage into this clean runtime.
COPY --from=publish /app/publish .

# Define the entrypoint – when the container starts, run the app.
ENTRYPOINT ["dotnet", "ProductDemo.dll"]