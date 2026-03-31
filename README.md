# Electric Vehicle Battery Exchange Management Website

Full-stack project for managing EV battery swap stations, drivers, station staff, subscriptions, payments, and support requests.

## Tech stack

- Backend: ASP.NET Core Web API, .NET 8, Entity Framework Core
- Database: PostgreSQL
- Frontend: React 19, Vite
- Auth: JWT Bearer
- API base path: `/api/v1`

## Repository structure

```text
electric-vehicle-battery-exchange-management-website/
|-- Docs/
|-- SWP_EVBatteryChangeStation_BE/
|   |-- EV_BatteryChangeStation/                # API project
|   |-- EV_BatteryChangeStation_Service/        # Business logic
|   |-- EV_BatteryChangeStation_Repository/     # EF Core + migrations
|   |-- EV_BatteryChangeStation_Common/         # Shared DTOs / helpers
|   `-- EV_BatteryChangeStation.sln
`-- SWP_EVBatteryChangeStation_FE/
    |-- src/
    `-- package.json
```

## Main modules

- Public station search and station detail
- Auth: register, OTP verify, login, logout, current profile
- Driver: vehicles, bookings, current subscription, payments, swap history, support requests, feedback
- Staff: station context, station bookings, battery inventory, swap completion, support response, payment record
- Admin: station management, user list, subscription plan management, support requests, revenue report

## Prerequisites

- .NET SDK 8.x
- Node.js 18+ and npm
- PostgreSQL 15+ running locally
- Git

Recommended:

- Visual Studio 2022 or VS Code
- DBeaver / pgAdmin for PostgreSQL

## Local setup

### 1. Clone repository

```bash
git clone <your-repo-url>
cd electric-vehicle-battery-exchange-management-website
```

### 2. Backend setup

Move to the backend folder:

```bash
cd SWP_EVBatteryChangeStation_BE
```

Install / restore packages:

```bash
dotnet restore
```

The solution is pinned to `.NET 8` in [`SWP_EVBatteryChangeStation_BE/global.json`](SWP_EVBatteryChangeStation_BE/global.json).

Configure PostgreSQL in [`SWP_EVBatteryChangeStation_BE/EV_BatteryChangeStation/appsettings.json`](SWP_EVBatteryChangeStation_BE/EV_BatteryChangeStation/appsettings.json):

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ev_batteryChangeStation;Username=postgres;Password=YOUR_PASSWORD"
}
```

Apply migrations and seed sample data:

```bash
dotnet ef database update --project EV_BatteryChangeStation_Repository --startup-project EV_BatteryChangeStation
```

Trust the local HTTPS developer certificate if needed:

```bash
dotnet dev-certs https --trust
```

Run backend:

```bash
dotnet run --project EV_BatteryChangeStation --launch-profile https
```

Backend URLs:

- Swagger: `https://localhost:7071/swagger`
- HTTPS API: `https://localhost:7071/api/v1`
- HTTP API: `http://localhost:5204/api/v1`

Note:

- The app redirects HTTP to HTTPS in development.
- Frontend should call the HTTPS URL directly to avoid redirect/CORS confusion.

### 3. Frontend setup

Open a second terminal:

```bash
cd SWP_EVBatteryChangeStation_FE
npm install
```

Create or update `.env`:

```env
VITE_API_BASE_URL=https://localhost:7071/api
```

Optional frontend keys currently used by the repo:

```env
VITE_GOOGLE_MAPS_API_KEY=YOUR_GOOGLE_MAPS_KEY
VITE_APP_VIETMAP_API_KEY=YOUR_VIETMAP_KEY
```

Run frontend:

```bash
npm run dev
```

Frontend URL:

- Vite default: `http://localhost:5173`

The backend already allows common local origins such as `localhost:5173` and `localhost:3000`.

## One-pass local run

1. Start PostgreSQL
2. Run backend migration:

```bash
cd SWP_EVBatteryChangeStation_BE
dotnet ef database update --project EV_BatteryChangeStation_Repository --startup-project EV_BatteryChangeStation
```

3. Start backend:

```bash
dotnet run --project EV_BatteryChangeStation --launch-profile https
```

4. Start frontend:

```bash
cd ..\SWP_EVBatteryChangeStation_FE
npm install
npm run dev
```

## Seed data

The current migrations seed sample data for:

- Roles, permissions, accounts
- Stations and station battery support
- Vehicle models, vehicles, batteries, battery history
- Subscription plans and user subscriptions
- Bookings, swap transactions, battery return inspections
- Inventory logs, payments
- Support requests, responses, feedback

This means after `dotnet ef database update`, the database is ready for demo/testing without manual inserts.

## Auth and OTP flow

Current local development behavior:

- Register creates a pending account and OTP
- OTP is logged in backend console/logs instead of relying on Gmail
- OTP must be verified before login
- Pending OTP is stored in memory, so restarting backend invalidates pending OTP sessions

Sample log:

```text
LOCAL OTP [REGISTER] Email=user@example.com OTP=123456 ExpiresAtUtc=...
```

Role note:

- Business docs use `DRIVER`
- Current seeded database uses `CUSTOMER` as the driver-facing role equivalent

## API documentation

Reference docs in [`Docs/`](Docs):

- [`Docs/BE_document_UserStory.md`](Docs/BE_document_UserStory.md)
- [`Docs/BE_document_APIResult.md`](Docs/BE_document_APIResult.md)
- [`Docs/BE_document_Structure.md`](Docs/BE_document_Structure.md)

Main controller groups:

- `/api/v1/auth`
- `/api/v1/stations`
- `/api/v1/driver`
- `/api/v1/staff`
- `/api/v1/admin`

## Troubleshooting

### Backend build fails because files are locked

If you see `MSB3027` / `MSB3021`, stop the running backend process first, then build again.

### Browser shows CORS error on `http://localhost:5204`

Use `https://localhost:7071/api` on the frontend. The backend redirects HTTP to HTTPS.

### PostgreSQL says database does not exist

Run:

```bash
dotnet ef database update --project EV_BatteryChangeStation_Repository --startup-project EV_BatteryChangeStation
```

### OTP is invalid

- Make sure you use the latest OTP from backend log
- Do not restart backend before verifying OTP
- If backend restarts, register again to generate a new OTP

### Login returns account not found

That usually means OTP verification has not completed successfully, so the account has not been created yet.

## Docker note

The current [`SWP_EVBatteryChangeStation_BE/docker-compose.yml`](SWP_EVBatteryChangeStation_BE/docker-compose.yml) is still aligned with the older SQL Server setup and is not the recommended path for the current local project state.

Recommended local workflow for this repo right now:

- PostgreSQL local
- `dotnet ef database update`
- `dotnet run --launch-profile https`
- `npm run dev`

## Security note

For team sharing or deployment, move sensitive values out of committed files:

- DB password
- JWT key
- email credentials
- third-party API keys

Use environment variables, `dotnet user-secrets`, or deployment secrets instead.
