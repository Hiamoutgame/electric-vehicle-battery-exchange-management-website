# Backend Deployment Guide

This guide explains how to deploy the EV Battery Change Station backend API using Docker.

## Prerequisites

- Docker installed on your machine
- Docker Hub account (or alternative container registry)
- A cloud provider account (Azure, AWS, Google Cloud, Railway, Render, etc.)
- SQL Server database (cloud-hosted or containerized)

## Table of Contents

1. [Local Testing with Docker](#local-testing-with-docker)
2. [Deployment Options](#deployment-options)
3. [Environment Configuration](#environment-configuration)
4. [Database Setup](#database-setup)
5. [Platform-Specific Guides](#platform-specific-guides)

---

## Local Testing with Docker

### Option 1: Using Docker Compose (Recommended for Local Testing)

1. **Update docker-compose.yml** with your database credentials:
   ```yaml
   environment:
     - SA_PASSWORD=YourStrong@Password123
   ```

2. **Update appsettings.json** or use environment variables for connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "server=sqlserver;database=EVBatterySwap;uid=sa;pwd=YourStrong@Password123;TrustServerCertificate=True"
   }
   ```

3. **Build and run**:
   ```bash
   docker-compose up -d
   ```

4. **Initialize database**:
   - Copy your SQL script to the container or run it manually
   - Or use Entity Framework migrations if available

5. **Access the API**:
   - API: http://localhost:8080
   - Swagger: http://localhost:8080/swagger

### Option 2: Using Docker Only

1. **Build the image**:
   ```bash
   docker build -t evbattery-api:latest .
   ```

2. **Run the container**:
   ```bash
   docker run -d \
     -p 8080:80 \
     -e ConnectionStrings__DefaultConnection="server=YOUR_DB_SERVER;database=EVBatterySwap;uid=YOUR_USER;pwd=YOUR_PASSWORD;TrustServerCertificate=True" \
     -e ASPNETCORE_ENVIRONMENT=Production \
     --name evbattery-api \
     evbattery-api:latest
   ```

---

## Deployment Options

### Recommended Platforms for .NET Backend:

1. **Azure Container Instances (ACI)** - Easy, serverless containers
2. **Azure App Service** - Managed platform with Docker support
3. **Railway** - Simple, developer-friendly
4. **Render** - Easy deployment with Docker
5. **AWS ECS/Fargate** - Scalable container service
6. **Google Cloud Run** - Serverless containers
7. **DigitalOcean App Platform** - Simple container hosting

---

## Environment Configuration

### Required Environment Variables

Create a `.env` file or set these in your deployment platform:

```env
# Database
ConnectionStrings__DefaultConnection=server=YOUR_DB_SERVER;database=EVBatterySwap;uid=YOUR_USER;pwd=YOUR_PASSWORD;TrustServerCertificate=True

# JWT Configuration
JwtConfig__Key=Hi18MvspEcBtmCVfaxD/rCGCzOotOf/wX3ntuh1tIN0=khongthemokhoa
JwtConfig__Issuer=App
JwtConfig__Audience=App
JwtConfig__ExpireMinutes=30

# Email Settings
EmailSettings__Email=your-email@gmail.com
EmailSettings__AppPassword=your-app-password

# VNPay Configuration
Vnpay__TmnCode=YOUR_TMN_CODE
Vnpay__HashSecret=YOUR_HASH_SECRET
Vnpay__BaseUrl=https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
Vnpay__ReturnUrl=https://your-backend-domain.com/api/VNPay/vnpay-return
Vnpay__Command=pay
Vnpay__CurrCode=VND
Vnpay__Version=2.1.0
Vnpay__Locale=vn

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:80
```

### Important Notes:

1. **CORS Configuration**: Update `Program.cs` to allow your Vercel frontend domain:
   ```csharp
   policy.WithOrigins("https://your-frontend.vercel.app")
   ```

2. **VNPay Return URL**: Update to your production backend URL

3. **Database**: Use a cloud-hosted SQL Server (Azure SQL, AWS RDS, etc.) for production

---

## Database Setup

### Option 1: Azure SQL Database (Recommended)

1. Create an Azure SQL Database
2. Get the connection string from Azure Portal
3. Update environment variable: `ConnectionStrings__DefaultConnection`
4. Run your SQL script to initialize the database

### Option 2: AWS RDS SQL Server

1. Create an RDS SQL Server instance
2. Get the connection string
3. Update environment variable
4. Run initialization script

### Option 3: Containerized SQL Server (Not Recommended for Production)

Use the `docker-compose.yml` for local development only.

---

## Platform-Specific Guides

### 1. Railway Deployment

1. **Install Railway CLI**:
   ```bash
   npm i -g @railway/cli
   railway login
   ```

2. **Initialize project**:
   ```bash
   railway init
   ```

3. **Add environment variables** in Railway dashboard

4. **Deploy**:
   ```bash
   railway up
   ```

5. **Add database** (PostgreSQL or MySQL available, or use external SQL Server)

### 2. Render Deployment

1. **Create a new Web Service** on Render
2. **Connect your GitHub repository**
3. **Configure**:
   - Build Command: `docker build -t evbattery-api .`
   - Start Command: `docker run -p 10000:80 evbattery-api`
   - Or use Dockerfile directly (Render auto-detects)

4. **Add environment variables** in Render dashboard
5. **Deploy**

### 3. Azure Container Instances

1. **Build and push to Azure Container Registry**:
   ```bash
   az acr build --registry YOUR_REGISTRY --image evbattery-api:latest .
   ```

2. **Create container instance**:
   ```bash
   az container create \
     --resource-group YOUR_RESOURCE_GROUP \
     --name evbattery-api \
     --image YOUR_REGISTRY.azurecr.io/evbattery-api:latest \
     --dns-name-label evbattery-api \
     --ports 80 \
     --environment-variables \
       ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING" \
       ASPNETCORE_ENVIRONMENT=Production
   ```

### 4. Azure App Service

1. **Build and push to Azure Container Registry**
2. **Create App Service** with Docker container
3. **Configure**:
   - Container image: `YOUR_REGISTRY.azurecr.io/evbattery-api:latest`
   - Port: 80
   - Add all environment variables

### 5. AWS ECS/Fargate

1. **Build and push to Amazon ECR**:
   ```bash
   aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin YOUR_ACCOUNT.dkr.ecr.us-east-1.amazonaws.com
   docker build -t evbattery-api .
   docker tag evbattery-api:latest YOUR_ACCOUNT.dkr.ecr.us-east-1.amazonaws.com/evbattery-api:latest
   docker push YOUR_ACCOUNT.dkr.ecr.us-east-1.amazonaws.com/evbattery-api:latest
   ```

2. **Create ECS task definition** with environment variables
3. **Create ECS service** or use Fargate

### 6. Google Cloud Run

1. **Build and push to Google Container Registry**:
   ```bash
   gcloud builds submit --tag gcr.io/YOUR_PROJECT/evbattery-api
   ```

2. **Deploy to Cloud Run**:
   ```bash
   gcloud run deploy evbattery-api \
     --image gcr.io/YOUR_PROJECT/evbattery-api \
     --platform managed \
     --region us-central1 \
     --allow-unauthenticated \
     --set-env-vars ConnectionStrings__DefaultConnection="YOUR_CONNECTION_STRING"
   ```

---

## Post-Deployment Checklist

- [ ] Update CORS settings in `Program.cs` with production frontend URL
- [ ] Update VNPay return URL to production backend URL
- [ ] Verify database connection
- [ ] Test API endpoints
- [ ] Configure SSL/HTTPS (most platforms do this automatically)
- [ ] Set up monitoring and logging
- [ ] Configure backup for database
- [ ] Update frontend API base URL to point to deployed backend

---

## Troubleshooting

### Container won't start
- Check logs: `docker logs evbattery-api`
- Verify environment variables are set correctly
- Check database connection string

### Database connection issues
- Verify SQL Server allows connections from your deployment platform
- Check firewall rules
- Verify credentials

### CORS errors
- Update CORS policy in `Program.cs` with exact frontend URL
- Ensure frontend is using correct backend URL

---

## Quick Start Commands

```bash
# Build image
docker build -t evbattery-api:latest .

# Test locally
docker run -p 8080:80 evbattery-api:latest

# Push to Docker Hub
docker tag evbattery-api:latest YOUR_USERNAME/evbattery-api:latest
docker push YOUR_USERNAME/evbattery-api:latest
```

---

## Support

For issues specific to your deployment platform, refer to their official documentation:
- [Railway Docs](https://docs.railway.app/)
- [Render Docs](https://render.com/docs)
- [Azure Container Instances](https://docs.microsoft.com/azure/container-instances/)
- [AWS ECS](https://docs.aws.amazon.com/ecs/)

