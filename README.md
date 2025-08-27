# Product API

A demo .NET 9 Web API project with full CI/CD pipeline.  
This repository is configured to use **GitVersion** for automatic semantic versioning and **GitHub Actions** for CI/CD, publishing Docker images to DockerHub.

---

## Features

- Clean architecture (.NET 9, EF Core, DTOs, Middleware, Validators, Logging, Unit Tests)
- Automatic semantic versioning using [GitVersion](https://gitversion.net/)
- GitHub Actions pipeline with build, test, and Docker publish
- Multi-branch strategy:
  - `main` -> Production (stable, tagged as `latest`)
  - `staging` -> Pre-production (tagged as `staging`)
  - `feature/*`, `fix/*`, `chore/*`, etc. -> Preview builds (versioned alpha/beta)

---

## CI/CD Workflow

1. **Versioning**  
   - GitVersion reads commit history, tags, and branches.  
   - Generates `SemVer` versions (e.g., `0.1.1`, `0.1.2-beta`, etc.).  

2. **Build & Test**  
   - GitHub Actions builds the .NET project.  
   - Runs unit tests for all branches.  

3. **Docker Images**  
   - On `staging`: pushed to DockerHub with `:semver` + `:staging`.  
   - On `main`: pushed to DockerHub with `:semver` + `:latest`.  

---

## DockerHub

Images are available at:  
docker pull mahmedk287/product-api:<tag>

**Latest stable release**
docker pull <your-dockerhub-username>/product-api:latest

**Specific version**
docker pull <your-dockerhub-username>/product-api:0.1.1

**Staging build**
docker pull <your-dockerhub-username>/product-api:staging


---

## Local Development

Clone the repository:
git clone https://github.com/m-ahmedk/product-api.git

cd product-api

Build and run locally:
- dotnet build
- dotnet run --project ProductDemo

Run with Docker:
docker build -t product-api:local -f ProductDemo/Dockerfile .
docker run -p 5000:80 product-api:local

---

## Tech Stack

- .NET 9 Web API
- Entity Framework Core
- FluentValidation
- AutoMapper
- Serilog
- GitVersion (semantic versioning)
- GitHub Actions (CI/CD)
- Docker + DockerHub

---

## License

This project is licensed under the MIT License.
