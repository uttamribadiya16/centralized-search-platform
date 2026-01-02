#!/bin/bash

echo "Starting Centralized Automotive Marketplace Platform with Docker Compose"
echo "====================================================================="

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "Error: Docker is not running. Please start Docker first."
    exit 1
fi

# Check if docker-compose is available
if ! command -v docker-compose > /dev/null 2>&1; then
    echo "Error: docker-compose is not installed. Please install docker-compose."
    exit 1
fi

echo "Building and starting all services..."
echo ""

# Build and start all services
docker-compose up --build -d

echo ""
echo "Waiting for services to start..."
sleep 30

echo ""
echo "====================================================================="
echo "All services are starting up..."
echo ""
echo "Services available at:"
echo "  Account Service Frontend:  http://localhost:3000"
echo "  Account Service Backend:   http://localhost:5001/api"
echo "  Offer Service Frontend:    http://localhost:3001" 
echo "  Offer Service Backend:     http://localhost:5002/api"
echo "  SQL Server Database:       localhost:1433"
echo ""
echo "Database credentials:"
echo "  Server: localhost,1433"
echo "  Username: sa"
echo "  Password: YourStrong@Passw0rd"
echo ""
echo "To stop all services: docker-compose down"
echo "To view logs: docker-compose logs -f [service-name]"
echo "====================================================================="