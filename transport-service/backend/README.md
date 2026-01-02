# Transport Service

Transport assignment and management service for carriers in the Centralized Search Platform.

## 🏗️ Architecture

- **Backend**: .NET Core 8.0 Web API
- **Database**: SQL Server
- **Message Broker**: RabbitMQ
- **Port**: Backend (5005)

## 📋 Features

- Carrier authentication and management
- Transport assignment to purchases
- Transport lifecycle management (Assigned, PickupScheduled, InTransit, Delivered, Cancelled)
- Integration with offer and purchase services
- Real-time transport updates
- Search integration via RabbitMQ events

## 🚀 Quick Start with Docker

### Using Docker Compose (Recommended)
```bash
# From project root
docker compose up transport-service-backend account-service-backend offer-service-backend purchase-service-backend sqlserver rabbitmq
```

### Individual Container Setup
```bash
# Start dependencies
docker run -d --name sqlserver -e ACCEPT_EULA=Y -e SA_PASSWORD=YourStrong@Passw0rd -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
docker run -d --name rabbitmq -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin123 -p 5672:5672 rabbitmq:3.12-management

# Build and run backend
cd transport-service/backend
docker build -t transport-service-api .
docker run -d -p 5005:5005 --name transport-service-api transport-service-api
```

## 🛠️ Local Development

### Backend Setup
```bash
cd transport-service/backend

# Install dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the API
dotnet run
```

## 📁 Project Structure

### Backend (`/backend`)
```
TransportService.API/
├── Controllers/
│   ├── AuthController.cs       # Carrier authentication
│   ├── TransportsController.cs # Transport management
│   ├── OffersController.cs     # View offers
│   └── PurchasesController.cs  # View purchases
├── Data/
│   └── TransportDbContext.cs   # Entity Framework context
├── Models/
│   ├── Transport.cs            # Transport entity
│   └── DTOs/                   # Data transfer objects
├── Services/
│   ├── AuthService.cs          # Authentication logic
│   ├── TransportService.cs     # Transport business logic
│   ├── OfferServiceClient.cs   # Offer service integration
│   ├── PurchaseServiceClient.cs # Purchase service integration
│   └── RabbitMQService.cs      # Event publishing
├── Migrations/                 # EF Core migrations
└── Program.cs                  # Application startup
```

## 🔧 Configuration

### Backend Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5005
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=transport-service;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
ACCOUNT_SERVICE_URL=http://localhost:5001
OFFER_SERVICE_URL=http://localhost:5002
PURCHASE_SERVICE_URL=http://localhost:5004
RABBITMQ_HOST=localhost
RABBITMQ_PORT=5672
RABBITMQ_USERNAME=admin
RABBITMQ_PASSWORD=admin123
```

## 📊 Database Schema

### Transports Table
| Column | Type | Description |
|--------|------|-------------|
| Id | uniqueidentifier | Primary key |
| CarrierId | uniqueidentifier | Carrier user ID |
| PurchaseId | uniqueidentifier | Related purchase ID |
| OfferId | uniqueidentifier | Related offer ID |
| BuyerId | uniqueidentifier | Buyer user ID |
| SellerId | uniqueidentifier | Seller user ID |
| Status | nvarchar(20) | Transport status |
| AssignedAt | datetime2 | Assignment timestamp |
| PickupScheduledAt | datetime2 | Scheduled pickup time |
| PickedUpAt | datetime2 | Actual pickup time |
| DeliveredAt | datetime2 | Delivery timestamp |
| TransportFee | decimal(18,2) | Transport cost |
| Notes | nvarchar(500) | Additional notes |
| PickupAddress | nvarchar(200) | Pickup location |
| DeliveryAddress | nvarchar(200) | Delivery location |
| CreatedAt | datetime2 | Creation timestamp |
| UpdatedAt | datetime2 | Last update timestamp |

### Transport Status Enum
- `Assigned` - Transport assigned to carrier
- `PickupScheduled` - Pickup scheduled
- `InTransit` - Vehicle in transit
- `Delivered` - Vehicle delivered
- `Cancelled` - Transport cancelled

## 🌐 API Endpoints

### Authentication
- `POST /api/auth/login` - Carrier login

### Transport Management
- `GET /api/transports` - List all transports with filtering
- `GET /api/transports/carrier/{carrierId}` - Get transports by carrier
- `GET /api/transports/{id}` - Get specific transport
- `POST /api/transports/assign?carrierId={id}` - Create transport assignment
- `PUT /api/transports/{id}` - Update transport status/details
- `DELETE /api/transports/{id}` - Delete transport

### External Data Access
- `GET /api/offers` - View all offers from offer-service
- `GET /api/offers/{id}` - Get specific offer details
- `GET /api/purchases` - View all purchases from purchase-service
- `GET /api/purchases/{id}` - Get specific purchase details

## 🔄 Event Publishing

Publishes RabbitMQ events for search indexing:
- `transport.created` - New transport assigned
- `transport.updated` - Transport status/details updated
- `transport.deleted` - Transport cancelled/removed

## 🧪 Testing

### API Testing with cURL
```bash
# Carrier login
curl -X POST http://localhost:5005/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "carrier@example.com",
    "password": "Password123!"
  }'

# Create transport assignment
curl -X POST "http://localhost:5005/api/transports/assign?carrierId=carrier-guid" \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseId": "purchase-guid-here",
    "transportFee": 500.00,
    "pickupScheduledAt": "2026-01-05T10:00:00Z",
    "pickupAddress": "Seller Address",
    "deliveryAddress": "Buyer Address",
    "notes": "Handle with care"
  }'

# Update transport status
curl -X PUT http://localhost:5005/api/transports/transport-guid \
  -H "Content-Type: application/json" \
  -d '{
    "status": 2,
    "pickedUpAt": "2026-01-05T10:30:00Z"
  }'
```

## 🐛 Troubleshooting

### Common Issues

1. **Authentication Failures**
   - Verify account-service is running
   - Check user has UserType = 3 (Carrier)

2. **Purchase Not Found**
   - Ensure purchase-service is running
   - Verify purchase exists and is valid for transport

3. **External Service Integration**
   ```bash
   # Check service connectivity
   curl http://localhost:5001/health  # Account Service
   curl http://localhost:5002/health  # Offer Service
   curl http://localhost:5004/health  # Purchase Service
   ```

### Health Check
```bash
curl http://localhost:5005/health
```

## 🔄 Integration

Integrates with:
- **Account Service**: Carrier authentication
- **Offer Service**: Vehicle offer details
- **Purchase Service**: Purchase information and validation
- **Search Service**: Transport indexing for search

## 📈 Monitoring

- Application logs: `docker logs transport-service-api`
- Transport assignment metrics
- External service integration health
- RabbitMQ message publishing
- Transport status progression tracking

## 🔐 Security

- Carrier-only access (UserType = 3)
- Input validation on all endpoints
- SQL injection protection via Entity Framework
- Secure integration with external services