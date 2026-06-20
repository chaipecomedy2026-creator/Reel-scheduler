# ReelSchedulerPro Setup Guide

## Prerequisites
- .NET 8 SDK
- PostgreSQL 14+
- Docker & Docker Compose (optional)

## Local Setup

### 1. Start Database

```bash
docker-compose up -d postgres redis
```

### 2. Install Dependencies

```bash
dotnet restore
```

### 3. Apply Migrations

```bash
cd src/ReelSchedulerPro.Api
dotnet ef database update
```

### 4. Configure Secrets (Development)

```bash
cd src/ReelSchedulerPro.Api
dotnet user-secrets set "Jwt:Secret" "your-secret-key-min-32-chars"
dotnet user-secrets set "Jwt:Issuer" "ReelSchedulerPro"
dotnet user-secrets set "Jwt:Audience" "ReelSchedulerProUsers"
```

### 5. Run API

```bash
cd src/ReelSchedulerPro.Api
dotnet run
```

API will be available at: `https://localhost:7001`
Swagger UI: `https://localhost:7001/swagger`

### 6. Run Worker (in another terminal)

```bash
cd src/ReelSchedulerPro.Worker
dotnet run
```

## Project Structure

```
ReelSchedulerPro/
├── src/
│   ├── ReelSchedulerPro.Api/           # ASP.NET Core Web API
│   ├── ReelSchedulerPro.Application/   # Business logic & services
│   ├── ReelSchedulerPro.Domain/        # Domain entities
│   ├── ReelSchedulerPro.Infrastructure # Data access, migrations
│   ├── ReelSchedulerPro.Shared/        # DTOs, validators, constants
│   └── ReelSchedulerPro.Worker/        # Background jobs & services
├── docker-compose.yml
└── ReelSchedulerPro.sln
```

## Features Implemented

✅ Solution structure with clean architecture
✅ Domain models for all core entities
✅ EF Core configuration and migrations
✅ PostgreSQL database setup
✅ Authentication skeleton (JWT)
✅ Shared DTOs and validators
✅ Application service interfaces
✅ Worker services (Posting, Health Check)
✅ API controllers with error handling
✅ Logging with Serilog
✅ CORS configuration

## Next Steps

1. **Authentication Implementation**: Implement JWT token generation and validation
2. **Instagram Integration**: Add OAuth and API integration
3. **Caption Generation**: Integrate OpenAI API
4. **Scheduler Logic**: Implement background job processing
5. **Frontend**: React/Next.js dashboard
6. **Real-time Updates**: SignalR integration
7. **Testing**: Unit and integration tests

## Environment Variables

```
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=reelscheduler;Username=postgres;Password=postgres
Jwt__Secret=your-secret-key-min-32-chars
Jwt__Issuer=ReelSchedulerPro
Jwt__Audience=ReelSchedulerProUsers
OpenAI__ApiKey=your-openai-key
Instagram__ClientId=your-instagram-app-id
Instagram__ClientSecret=your-instagram-app-secret
```

## Troubleshooting

### Database Connection Error
- Ensure PostgreSQL is running: `docker ps`
- Check connection string in appsettings.json

### Migration Issues
```bash
dotnet ef database drop -f
dotnet ef database update
```

## References
- [Clean Architecture Guide](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/)
- [EF Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
