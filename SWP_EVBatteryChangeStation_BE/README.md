# SWP_EVBatteryChangeStation_BE

ASP.NET Core Web API backend for the EV battery exchange management system.

## Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT authentication

## Run locally

From this folder:

```bash
dotnet restore
dotnet ef database update --project EV_BatteryChangeStation_Repository --startup-project EV_BatteryChangeStation
dotnet dev-certs https --trust
dotnet run --project EV_BatteryChangeStation --launch-profile https
```

Endpoints:

- Swagger: `https://localhost:7071/swagger`
- API base: `https://localhost:7071/api/v1`

## Connection string

Edit [`EV_BatteryChangeStation/appsettings.json`](EV_BatteryChangeStation/appsettings.json):

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ev_batteryChangeStation;Username=postgres;Password=YOUR_PASSWORD"
}
```

## Notes

- Migrations already include sample seed data.
- Local OTP for registration is written to backend logs.
- The current `docker-compose.yml` still reflects the older SQL Server path and is not the recommended setup for the repo in its current state.

For the full project setup guide, use the root [`../README.md`](../README.md).
