# ---- Base runtime image ----
# Only the ASP.NET runtime lightweight and secure.
# App runs inside container on port 8080
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080   

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy ONLY the API project and solution.
# We deliberately skip test projects here (tests run in CI/CD, not in the container).
COPY ProductDemo/ProductDemo.csproj ProductDemo/
COPY *.sln ./

# Restore just the API dependencies.
RUN dotnet restore ProductDemo/ProductDemo.csproj

# Copy the rest of the codebase.
COPY . .

# Build the API project in Release mode.
WORKDIR /src/ProductDemo
RUN dotnet build -c Release -o /app/build

# ---- Publish stage ----
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---- Final runtime image ----
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductDemo.dll"]