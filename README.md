# Centralized Search Platform

A comprehensive microservices-based platform for vehicle trading with search capabilities, built with .NET Core 8.0, React, and Docker.

## 🏗️ Architecture Overview

The platform consists of the following services:

- **Account Service** (Port 5001/3000): User management and authentication
- **Offer Service** (Port 5002/3001): Vehicle listing management for sellers
- **Purchase Service** (Port 5004/3002): Purchase management for buyers
- **Transport Service** (Port 5005): Transport assignment for carriers
- **Search Service** (Port 5003): Elasticsearch-powered search across all data
- **SQL Server** (Port 1433): Primary database
- **RabbitMQ** (Port 5672/15672): Message broker for event-driven architecture
- **Elasticsearch** (Port 9200/9300): Search engine

## 🚀 Quick Start

### Prerequisites
- Docker Desktop
- Docker Compose
- 8GB+ RAM recommended

### 1. Clone Repository
```bash
git clone <repository-url>
cd centralized-search-platform
```

### 2. Start All Services
```bash
# Start all services
docker compose up -d

# View logs
docker compose logs -f

# Check service status
docker compose ps
```

### 3. Access Applications
- **Account Management**: http://localhost:3000
- **Seller Dashboard**: http://localhost:3001
- **Buyer Dashboard**: http://localhost:3002
- **RabbitMQ Management**: http://localhost:15672 (admin/admin123)
- **Elasticsearch**: http://localhost:9200

### 4. API Endpoints
- **Account API**: http://localhost:5001/api
- **Offer API**: http://localhost:5002/api
- **Search API**: http://localhost:5003/api
- **Purchase API**: http://localhost:5004/api
- **Transport API**: http://localhost:5005/api

## 🛠️ Development

### Individual Service Setup
Each service can be run independently. See service-specific README files:

- [Account Service](./account-service/README.md)
- [Offer Service](./offer-service/README.md)
- [Purchase Service](./purchase-service/README.md)
- [Transport Service](./transport-service/README.md)
- [Search Service](./search-service/README.md)

### Docker Commands
```bash
# Build and start services
docker compose up --build

# Stop all services
docker compose down

# Remove volumes (reset data)
docker compose down -v

# View specific service logs
docker compose logs -f <service-name>

# Restart specific service
docker compose restart <service-name>

# Scale services (if needed)
docker compose up --scale offer-service-backend=2
```

## 📊 User Types & Access

| UserType | Role | Access |
|----------|------|---------|
| 1 | Seller | Create/manage offers, view purchases |
| 2 | Buyer | Browse offers, create purchases |
| 3 | Carrier | Manage transport assignments |
| 4 | Agent | Administrative access |

## 🔄 Event-Driven Architecture

Services communicate via RabbitMQ events:
- `offer.*` - Offer lifecycle events
- `purchase.*` - Purchase lifecycle events
- `transport.*` - Transport lifecycle events

All events are consumed by the Search Service for Elasticsearch indexing.

## 🗄️ Database Schema

Each service has its own database:
- `account-service` - User accounts and authentication
- `offer-service` - Vehicle offers and listings
- `purchase-service` - Purchase transactions
- `transport-service` - Transport assignments

## 🔍 Search Capabilities

The Search Service provides:
- Full-text search across offers, purchases, and transports
- Role-based filtering
- Real-time indexing via event consumption
- RESTful search API

## 🐛 Troubleshooting

### Common Issues
1. **Port Conflicts**: Ensure ports 3000-3002, 5001-5005, 1433, 5672, 9200 are available
2. **Memory Issues**: Increase Docker memory allocation to 8GB+
3. **Service Dependencies**: Wait for health checks to pass before accessing services

### Health Checks
```bash
# Check all services
curl http://localhost:5001/health  # Account Service
curl http://localhost:5002/health  # Offer Service
curl http://localhost:5003/api/search/health  # Search Service
curl http://localhost:5004/health  # Purchase Service
curl http://localhost:5005/health  # Transport Service
```

### Reset Everything
```bash
docker compose down -v
docker system prune -a
docker compose up --build
```

## 🧪 Testing

### API Testing
Use the included Postman collection:
```
Centralized-Search-Platform.postman_collection.json
```

### Sample Data Flow
1. Create seller account (UserType: 1)
2. Login and create vehicle offer
3. Create buyer account (UserType: 2)
4. Login and purchase vehicle
5. Create carrier account (UserType: 3)
6. Login and assign transport
7. Search for data via Search Service

## 📈 Monitoring

- **RabbitMQ**: Monitor message queues at http://localhost:15672
- **Elasticsearch**: Check cluster health at http://localhost:9200/_cluster/health
- **Logs**: Use `docker compose logs -f <service>` for real-time logs

## 🔧 Configuration

Environment variables are configured in docker-compose.yml:
- Database connections
- Service URLs
- RabbitMQ settings
- Elasticsearch configuration

## 📝 API Documentation

Once services are running, access Swagger documentation:
- Account API: http://localhost:5001/swagger
- Offer API: http://localhost:5002/swagger
- Purchase API: http://localhost:5004/swagger
- Transport API: http://localhost:5005/swagger

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Make changes
4. Test with Docker Compose
5. Submit pull request

## 📄 License

This project is licensed under the MIT License.