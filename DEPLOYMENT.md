# 🚀 Step-by-Step Deployment Guide

## Prerequisites
- EC2 instance (t3.medium or larger)
- Docker & Docker Compose installed

## Step 1: Create Docker Network
```bash
docker network create ecommerce-network
```

## Step 2: Start SQL Server Container
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Sujith@!23" \
  --name sqlserver --hostname sqlserver \
  --network ecommerce-network \
  --restart unless-stopped -d mcr.microsoft.com/mssql/server:2019-latest
```

## Step 3: Create Database
```bash
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -Q "
CREATE DATABASE Vendor_Ecommerce;
CREATE LOGIN ecommerce_user WITH PASSWORD = 'Sujith@!23';
USE Vendor_Ecommerce;
CREATE USER ecommerce_user FOR LOGIN ecommerce_user;
ALTER ROLE db_owner ADD MEMBER ecommerce_user;
"
```

## Step 4: Configure Environment Files

**A. Create Backend .env file in project root:**
```bash
# In the main project directory (where docker-compose.yml is)
cp .env.example .env
```

Edit `.env` file with:
```bash
# Database Configuration (use sqlserver as hostname)
DB_CONNECTION_STRING=Server=sqlserver,1433;Database=Vendor_Ecommerce;User Id=ecommerce_user;Password=YourStrongPassword;TrustServerCertificate=true;

# JWT Configuration
JWT_SECRET_KEY=YourSuperSecretKeyThatIsAtLeast32CharactersLong123456789

# Email Configuration
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your_email@gmail.com
SMTP_PASSWORD=your_app_password
FROM_EMAIL=noreply@yourdomain.com

# Razorpay Configuration
RAZORPAY_KEY_ID=your_razorpay_key_id
RAZORPAY_KEY_SECRET=your_razorpay_key_secret
```

**B. Update Frontend .env.production:**
```bash
# Edit Frontend/ecommerce/.env.production
VITE_API_BASE_URL=http://YOUR_EC2_IP:5108/api
```

## Step 5: Update Docker Compose

Create/Update `docker-compose.yml`:
```yaml
version: '3.8'
services:
  backend:
    build: ./Backend
    ports:
      - "5108:5108"
    networks:
      - ecommerce-network
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=Vendor_Ecommerce;User Id=ecommerce_user;Password=Sujith@!23;TrustServerCertificate=true;
  
  frontend:
    build: ./Frontend/ecommerce
    ports:
      - "80:80"
    networks:
      - ecommerce-network
    depends_on:
      - backend

networks:
  ecommerce-network:
    external: true
```

## Step 6: Build and Deploy

```bash
# Build and start containers
docker-compose up -d --build

# Check all containers
docker ps

# View logs
docker-compose logs -f
```

## Step 7: Configure EC2 Security Group

Open these ports:
- **80** - Frontend (HTTP)
- **5108** - Backend API
- **22** - SSH

## Step 8: Test Deployment

```bash
# Check all containers are running
docker ps

# Test frontend
curl http://YOUR_EC2_IP

# Test backend API
curl http://YOUR_EC2_IP:5108/api/health

# Check network connectivity
docker exec backend ping sqlserver
```

## Complete Command Summary

```bash
# 1. Create network
docker network create ecommerce-network

# 2. Start SQL Server
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Sujith@!23" \
  --name sqlserver --hostname sqlserver \
  --network ecommerce-network \
  --restart unless-stopped -d mcr.microsoft.com/mssql/server:2019-latest

# 3. Create database
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -Q "
CREATE DATABASE Vendor_Ecommerce;
CREATE LOGIN ecommerce_user WITH PASSWORD = 'Sujith@!23';
USE Vendor_Ecommerce;
CREATE USER ecommerce_user FOR LOGIN ecommerce_user;
ALTER ROLE db_owner ADD MEMBER ecommerce_user;
"

# 4. Deploy application
docker-compose up -d --build

# 5. Check status
docker ps
```

## Troubleshooting

### Container Issues
```bash
# Restart containers
docker-compose restart

# Rebuild containers
docker-compose up -d --build --force-recreate

# Check container logs
docker-compose logs backend
```

### Common Issues

**Database Connection:**
```bash
# Check SQL Server is running
docker ps | grep sqlserver

# Test network connectivity
docker exec backend ping sqlserver

# Check SQL Server logs
docker logs sqlserver
```

**Container Issues:**
```bash
# Restart all containers
docker-compose restart

# Rebuild containers
docker-compose up -d --build --force-recreate

# Check logs
docker-compose logs backend
docker-compose logs frontend
```

### Frontend API Issues
- Verify backend container is running
- Check API URL in .env.production
- Ensure CORS is configured properly