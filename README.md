# Electric Vehicle Battery Exchange Management System

A full-stack web application for managing electric vehicle battery exchange stations, built with .NET 8.0 backend and React frontend.

## Project Description

**Tech Stack:** React (Vite) frontend, .NET 8.0 (C#) backend with Entity Framework Core, SQL Server, and VietMap API integration. **Key Contributions:** Developed frontend React components, integrated VietMap API for interactive maps and route visualization, and configured CORS policies and port settings in the C# backend. **Learning Outcomes:** Gained experience in API setup and consumption, React component development with asset/template management, and backend configuration including database connections, JWT authentication, and CORS middleware.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Backend Setup (C# .NET 8.0)](#backend-setup-c-net-80)
- [Frontend Setup (React + Vite)](#frontend-setup-react--vite)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

Before you begin, ensure you have the following installed:

### For Backend:
- **.NET SDK 8.0** or later ([Download](https://dotnet.microsoft.com/download))
- **SQL Server 2019+** or **SQL Server Express** ([Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads))
- **Visual Studio 2022** or **Visual Studio Code** (optional, but recommended)
- **Docker Desktop** (optional, for containerized setup)

### For Frontend:
- **Node.js 18+** and **npm** ([Download](https://nodejs.org/))
- **Git** ([Download](https://git-scm.com/))

---

## Project Structure

```
electric-vehicle-battery-exchange-management-website/
├── SWP_EVBatteryChangeStation_BE/     # Backend (.NET 8.0)
│   ├── EV_BatteryChangeStation/      # Main API project
│   ├── EV_BatteryChangeStation_Repository/  # Data access layer
│   ├── EV_BatteryChangeStation_Service/     # Business logic layer
│   ├── EV_BatteryChangeStation_Common/      # Shared utilities
│   ├── Database/                     # SQL scripts
│   └── docker-compose.yml            # Docker configuration
│
└── SWP_EVBatteryChangeStation_FE/     # Frontend (React + Vite)
    ├── src/                          # Source code
    ├── public/                       # Static assets
    └── package.json                  # Dependencies
```

---

## Backend Setup (C# .NET 8.0)

### Option 1: Using Docker Compose (Recommended for Quick Start)

This method sets up both SQL Server and the backend API in containers.

1. **Navigate to the backend directory:**
   ```bash
   cd SWP_EVBatteryChangeStation_BE
   ```

2. **Update database credentials in `docker-compose.yml`** (if needed):
   ```yaml
   environment:
     - SA_PASSWORD=YourStrong@Password123
   ```

3. **Build and run containers:**
   ```bash
   docker-compose up -d
   ```

4. **Initialize the database:**
   - The database will be created automatically
   - Run the SQL script from `Database/EVBatterySwap.sql` to create tables and initial data
   - You can execute it using SQL Server Management Studio (SSMS) or Azure Data Studio

5. **Access the API:**
   - API: http://localhost:8080
   - Swagger UI: http://localhost:8080/swagger

### Option 2: Direct .NET Run (Local Development)

1. **Navigate to the backend directory:**
   ```bash
   cd SWP_EVBatteryChangeStation_BE
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Configure the database connection:**
   - Open `EV_BatteryChangeStation/appsettings.json`
   - Update the `ConnectionStrings` section with your SQL Server credentials:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "server=(local); database=EVBatterySwap; uid=sa; pwd=YOUR_PASSWORD; TrustServerCertificate=True"
   }
   ```

4. **Set up the database:**
   - Ensure SQL Server is running
   - Create the database by running the script: `Database/EVBatterySwap.sql`
   - Or use SQL Server Management Studio to execute the script

5. **Run the application:**
   ```bash
   cd EV_BatteryChangeStation
   dotnet run
   ```

6. **Access the API:**
   - API: http://localhost:5204
   - Swagger UI: http://localhost:5204/swagger

### Option 3: Using Visual Studio

1. **Open the solution:**
   - Open `SWP_EVBatteryChangeStation_BE/EV_BatteryChangeStation.sln` in Visual Studio 2022

2. **Set startup project:**
   - Right-click on `EV_BatteryChangeStation` project → Set as Startup Project

3. **Configure connection string:**
   - Update `appsettings.json` with your database connection string

4. **Run the application:**
   - Press `F5` or click the Run button
   - Swagger UI will open automatically

---

## Frontend Setup (React + Vite)

1. **Navigate to the frontend directory:**
   ```bash
   cd SWP_EVBatteryChangeStation_FE
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Create environment file:**
   - Create a `.env` file in the `SWP_EVBatteryChangeStation_FE` directory
   - Add the following configuration:
   ```env
   VITE_API_BASE_URL=http://localhost:5204
   ```
   
   **Note:** If you're using Docker for the backend, use:
   ```env
   VITE_API_BASE_URL=http://localhost:8080
   ```

4. **Start the development server:**
   ```bash
   npm run dev
   ```

5. **Access the application:**
   - Frontend: http://localhost:3000

### Build for Production

To create a production build:

```bash
npm run build
```

The optimized files will be in the `dist` directory.

---

## Database Setup

### Manual Setup

1. **Install SQL Server:**
   - Download and install SQL Server 2019 or later
   - Or use SQL Server Express (free)

2. **Create the database:**
   - Open SQL Server Management Studio (SSMS) or Azure Data Studio
   - Connect to your SQL Server instance
   - Execute the script: `SWP_EVBatteryChangeStation_BE/Database/EVBatterySwap.sql`
   - This will create the database and all required tables

3. **Verify the connection:**
   - Update the connection string in `appsettings.json`
   - Test the connection by running the backend

### Using Docker (SQL Server Container)

If you're using Docker Compose, SQL Server will be automatically set up. You still need to run the SQL script to create tables:

1. **Connect to the containerized SQL Server:**
   ```bash
   docker exec -it evbattery-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123
   ```

2. **Or use SSMS/Azure Data Studio:**
   - Server: `localhost,1433`
   - Username: `sa`
   - Password: `YourStrong@Password123` (or the password you set in docker-compose.yml)

3. **Execute the SQL script** to create tables and initial data

---

## Configuration

### Backend Configuration (`appsettings.json`)

Key configuration sections:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=(local); database=EVBatterySwap; uid=sa; pwd=YOUR_PASSWORD; TrustServerCertificate=True"
  },
  "JwtConfig": {
    "Key": "YOUR_JWT_SECRET_KEY",
    "Issuer": "App",
    "Audience": "App",
    "ExpireMinutes": 30
  },
  "EmailSettings": {
    "Email": "your-email@gmail.com",
    "AppPassword": "your-app-password"
  },
  "AllowedOrigins": [
    "http://localhost:3000"
  ],
  "Vnpay": {
    "TmnCode": "YOUR_TMN_CODE",
    "HashSecret": "YOUR_HASH_SECRET",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "http://localhost:5204/api/VNPay/vnpay-return"
  }
}
```

### Frontend Configuration (`.env`)

```env
VITE_API_BASE_URL=http://localhost:5204
```

**Important:** 
- For Docker backend, use `http://localhost:8080`
- For production, update with your production API URL

---

## Running the Application

### Complete Setup Flow

1. **Start the database:**
   - Option A: Use Docker Compose (includes SQL Server)
   - Option B: Ensure SQL Server is running locally

2. **Initialize the database:**
   - Run the SQL script: `Database/EVBatterySwap.sql`

3. **Start the backend:**
   ```bash
   cd SWP_EVBatteryChangeStation_BE/EV_BatteryChangeStation
   dotnet run
   ```
   - Or use Docker: `docker-compose up -d` (from backend directory)

4. **Start the frontend:**
   ```bash
   cd SWP_EVBatteryChangeStation_FE
   npm run dev
   ```

5. **Access the application:**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5204 (or http://localhost:8080 for Docker)
   - Swagger UI: http://localhost:5204/swagger (or http://localhost:8080/swagger for Docker)

---

## API Documentation

Once the backend is running, you can access the interactive API documentation:

- **Swagger UI:** http://localhost:5204/swagger (or http://localhost:8080/swagger for Docker)

The Swagger UI provides:
- Complete API endpoint documentation
- Interactive API testing
- Request/response schemas
- Authentication testing (JWT Bearer tokens)

### Authentication

The API uses JWT (JSON Web Tokens) for authentication. To test protected endpoints:

1. Use the `/api/Authen/login` endpoint to get a token
2. Click the "Authorize" button in Swagger UI
3. Enter: `Bearer YOUR_TOKEN_HERE`
4. Now you can test protected endpoints

---

## Troubleshooting

### Backend Issues

**Problem: Cannot connect to database**
- Verify SQL Server is running
- Check the connection string in `appsettings.json`
- Ensure the database `EVBatterySwap` exists
- For Docker, check if the SQL Server container is healthy: `docker ps`

**Problem: Port already in use**
- Change the port in `launchSettings.json` or `docker-compose.yml`
- Or stop the process using the port

**Problem: JWT authentication fails**
- Verify `JwtConfig` settings in `appsettings.json`
- Ensure the JWT key is properly configured

### Frontend Issues

**Problem: Cannot connect to backend API**
- Verify the backend is running
- Check `VITE_API_BASE_URL` in `.env` file
- Ensure CORS is configured correctly in backend `appsettings.json`
- Check browser console for CORS errors

**Problem: Dependencies installation fails**
- Clear npm cache: `npm cache clean --force`
- Delete `node_modules` and `package-lock.json`, then run `npm install` again
- Ensure Node.js version is 18 or higher

**Problem: Build errors**
- Check Node.js version: `node --version` (should be 18+)
- Update dependencies: `npm update`
- Clear Vite cache: Delete `.vite` folder if it exists

### Database Issues

**Problem: SQL script execution fails**
- Ensure you're connected to the correct SQL Server instance
- Check SQL Server version compatibility
- Verify you have sufficient permissions to create databases

**Problem: Tables not created**
- Re-run the SQL script
- Check for error messages in SQL Server Management Studio
- Verify the database `EVBatterySwap` exists

---

## Additional Resources

- **Backend Deployment Guide:** See `SWP_EVBatteryChangeStation_BE/DEPLOYMENT_GUIDE.md`
- **Backend README:** See `SWP_EVBatteryChangeStation_BE/README.md`
- **.NET Documentation:** https://docs.microsoft.com/en-us/dotnet/
- **React Documentation:** https://react.dev/
- **Vite Documentation:** https://vitejs.dev/

---

## Support

For issues or questions, please refer to the project documentation or contact the development team.
