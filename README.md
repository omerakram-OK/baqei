# Baqei - .NET 8 Web API (Clean Architecture) with PostgreSQL, MongoDB, Redis, RabbitMQ

## What is included
- Solution: Baqei.sln
- Projects:
  - Baqei.Domain (entities, interfaces)
  - Baqei.Application (DTOs, services)
  - Baqei.Infrastructure (EF Core Postgres, Mongo, Redis, RabbitMQ)
  - Baqei.Api (Web API, controllers, Swagger)

## Quick start
1. Install .NET 8 SDK: https://dotnet.microsoft.com/
2. Ensure PostgreSQL, MongoDB, Redis, RabbitMQ are running locally or update connection strings in `Baqei.Api/appsettings.json`.
3. From solution folder:
   - `dotnet restore`
   - `dotnet build`
4. Add EF migrations (install dotnet-ef tool if needed):
   - `dotnet tool install --global dotnet-ef`
   - `dotnet ef migrations add Initial -p Baqei.Infrastructure -s Baqei.Api`
   - `dotnet ef database update -p Baqei.Infrastructure -s Baqei.Api`
5. Run API:
   - `dotnet run --project Baqei.Api`

## Notes
- RabbitMQ: This project uses the raw RabbitMQ.Client. The `RabbitMqService` performs basic publish and consume.
- MongoDB: A simple `UserMongoRepository` is included; it's separate from the Postgres repository.
- Redis: `RedisCacheService` wraps StackExchange.Redis for simple get/set operations.

## Next steps you might want to add
- Health checks
- Validation (FluentValidation)
- Authentication (JWT)
- Background worker for message processing (IHostedService)
