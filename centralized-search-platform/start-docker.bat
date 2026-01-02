@echo off
echo Starting Centralized Automotive Marketplace Platform with Docker Compose
echo =====================================================================

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo Error: Docker is not running. Please start Docker first.
    pause
    exit /b 1
)

REM Check if docker-compose is available
docker-compose --version >nul 2>&1
if errorlevel 1 (
    echo Error: docker-compose is not installed. Please install docker-compose.
    pause
    exit /b 1
)

echo Building and starting all services...
echo.

REM Build and start all services
docker-compose up --build -d

echo.
echo Waiting for services to start...
timeout /t 30 /nobreak >nul

echo.
echo =====================================================================
echo All services are starting up...
echo.
echo Services available at:
echo   Account Service Frontend:  http://localhost:3000
echo   Account Service Backend:   http://localhost:5001/api
echo   Offer Service Frontend:    http://localhost:3001
echo   Offer Service Backend:     http://localhost:5002/api
echo   SQL Server Database:       localhost:1433
echo.
echo Database credentials:
echo   Server: localhost,1433
echo   Username: sa
echo   Password: YourStrong@Passw0rd
echo.
echo To stop all services: docker-compose down
echo To view logs: docker-compose logs -f [service-name]
echo =====================================================================
pause