# 🧾 Discount Service

A lightweight, containerized Discount microservice built with .NET 8 using gRPC, Entity Framework Core, and SQLite. Designed with clean architecture principles and functional programming practices, the service is optimized for simplicity, maintainability, and performance.

---

## 🚀 Features

- ✅ Built with .NET 8 (LTS)
- ✅ Clean, layered architecture (API, Application, Domain, Infrastructure)
- ✅ gRPC services using Protocol Buffers
- ✅ Custom interceptors for centralized exception handling and correlation ID propagation
- ✅ EF Core with SQLite using code-first migrations
- ✅ Health checks for service and database monitoring
- ✅ Structured logging via Serilog
- ✅ Functional programming style with CSharpFunctionalExtensions
- ✅ Dockerized with Docker Compose support
- ✅ Unit and integration tests with NUnit, FluentAssertions, and NSubstitute

---

## 🛠 Tech Stack

| Purpose           | Technology                  |
| ----------------- | --------------------------- |
| Language          | C# (.NET 8)                 |
| API Protocol      | gRPC with Protobuf          |
| ORM               | Entity Framework Core       |
| Database          | SQLite                      |
| Architecture      | N-tier (Clean Architecture) |
| Containerization  | Docker & Docker Compose     |
| Tracing           | Correlation ID Middleware   |
| Logging           | Serilog                     |
| Functional Style  | CSharpFunctionalExtensions  |
| Testing Framework | NUnit                       |
| Assertions        | FluentAssertions            |
| Mocking           | NSubstitute                 |

---

## 🧰 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)

### 📡 Run with Docker

```bash
docker-compose up --build

```

### 🚀 Run Locally

```bash

dotnet run --DiscountEngine.Grpc

```

💡 Note: The migration files will be created under --> src/DiscountEngine.Infrastructure/Data/Migrations

## 📦 Migrations

### 🔧 Create a Migration (run from project root)

```bash

dotnet ef migrations add InitialCreate --project .\src\DiscountEngine.Infrastructure --startup-project .\src\DiscountEngine.Grpc --context DiscountDbContext --output-dir Data\Migrations
```

### 🚀 Apply the Migration

```bash
dotnet ef database update --project .\src\DiscountEngine.Infrastructure --startup-project .\src\DiscountEngine.Grpc --context DiscountDbContext

```

✅ You can skip this step if you're just running the app — it will apply the latest migrations automatically.

## 📝 Logging

- Structured logging is provided by Serilog with configuration via appsettings.json. Output includes timestamps, correlation IDs.

## 📈 Planned Improvements

- 🔐 **Authentication & Authorization**

  - Integrate JWT or API key-based security for gRPC endpoints to protect the service.

- 🔁 **Retry & Circuit Breaker Policies**

  - Implement Retry and Circuit Breaker policies with Polly

- 🗃️ **Unit & Integration Tests improvements**

  - Increase Unit Testing & Integration Testing Coverage
