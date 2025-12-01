# SWP_EVBatteryChangeStation_BE

Backend API for EV Battery Change Station system built with .NET 8.0

## Quick Start

### Local Development

1. **Using Docker Compose** (includes SQL Server):
   ```bash
   docker-compose up -d
   ```

2. **Using Docker only**:
   ```bash
   docker build -t evbattery-api:latest .
   docker run -p 8080:80 evbattery-api:latest
   ```

3. **Direct .NET run**:
   ```bash
   cd EV_BatteryChangeStation
   dotnet run
   ```

## Deployment

📖 **See [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) for detailed deployment instructions.**

The backend can be deployed to:
- Railway
- Render
- Azure Container Instances / App Service
- AWS ECS/Fargate
- Google Cloud Run
- DigitalOcean App Platform

## Configuration

- Update `appsettings.json` or use environment variables
- Configure CORS for your frontend domain (Vercel)
- Set up SQL Server database (Azure SQL, AWS RDS, etc.)
- Configure VNPay return URLs for production

## API Documentation

Once running, access Swagger UI at:
- Local: http://localhost:5204/swagger
- Docker: http://localhost:8080/swagger