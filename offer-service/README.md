# Offer Service

Vehicle listing management service for sellers in the Centralized Search Platform.

## 🏗️ Architecture

- **Backend**: .NET Core 8.0 Web API
- **Frontend**: React 18.2.0
- **Database**: SQL Server
- **Message Broker**: RabbitMQ
- **Port**: Backend (5002), Frontend (3001)

## 📋 Features

- Vehicle offer creation and management
- Seller authentication integration
- Offer lifecycle management (Active, Sold, Expired)
- Image upload and management
- Search integration via RabbitMQ events
- Real-time offer updates

## 🚀 Quick Start with Docker

### Using Docker Compose (Recommended)
```bash
# From project root
docker compose up offer-service-backend offer-service-frontend account-service-backend sqlserver rabbitmq
```

### Individual Container Setup
```bash
# Start dependencies
docker run -d --name sqlserver -e ACCEPT_EULA=Y -e SA_PASSWORD=YourStrong@Passw0rd -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
docker run -d --name rabbitmq -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin123 -p 5672:5672 -p 15672:15672 rabbitmq:3.12-management

# Build and run backend
cd offer-service/backend/OfferService.API
docker build -t offer-service-api .
docker run -d -p 5002:5002 --name offer-service-api offer-service-api

# Build and run frontend
cd ../../frontend
docker build -t offer-service-web .
docker run -d -p 3001:3001 --name offer-service-web offer-service-web
```

## 🛠️ Local Development

### Backend Setup
```bash
cd offer-service/backend/OfferService.API

# Install dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the API
dotnet run
```

### Frontend Setup
```bash
cd offer-service/frontend

# Install dependencies
npm install

# Start development server
npm start
```

## 📁 Project Structure

### Backend (`/backend/OfferService.API`)
```
OfferService.API/
├── Controllers/
│   ├── AuthController.cs       # Seller authentication
│   └── OffersController.cs     # Offer management
├── Data/
│   └── OfferDbContext.cs       # Entity Framework context
├── Models/
│   ├── Offer.cs                # Offer entity
│   └── DTOs/                   # Data transfer objects
├── Services/
│   ├── AuthService.cs          # Authentication logic
│   ├── OfferService.cs         # Offer business logic
│   └── RabbitMQService.cs      # Event publishing
├── Migrations/                 # EF Core migrations
└── Program.cs                  # Application startup
```

### Frontend (`/frontend`)
```
src/
├── components/
│   ├── OfferCard.js           # Offer display component
│   ├── OfferForm.js           # Offer creation form
│   └── ImageUpload.js         # Image upload component
├── pages/
│   ├── LoginPage.js           # Seller login
│   ├── OfferListPage.js       # Offer management
│   └── CreateOfferPage.js     # New offer creation
├── services/
│   └── apiService.js          # API integration
└── styles/
```

## 🔧 Configuration

### Backend Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5002
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=offer-service;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
ConnectionStrings__RabbitMQ=amqp://admin:admin123@localhost:5672/
AccountServiceBaseUrl=http://localhost:5001
```

### Frontend Environment Variables
```bash
REACT_APP_API_URL=http://localhost:5002/api
REACT_APP_ACCOUNT_API_URL=http://localhost:5001/api
REACT_APP_SEARCH_API_URL=http://localhost:5003/api
```

## 📊 Database Schema

### Offers Table
| Column | Type | Description |
|--------|------|-------------|
| Id | uniqueidentifier | Primary key |
| SellerId | uniqueidentifier | Seller user ID |
| VIN | nvarchar(17) | Vehicle identification number |
| Make | nvarchar(50) | Vehicle manufacturer |
| Model | nvarchar(50) | Vehicle model |
| Year | int | Manufacturing year |
| OfferAmount | decimal(18,2) | Asking price |
| Condition | nvarchar(20) | Vehicle condition |
| Status | nvarchar(20) | Offer status |
| Address | nvarchar(200) | Vehicle location |
| CreatedAt | datetime2 | Creation timestamp |
| UpdatedAt | datetime2 | Last update timestamp |

## 🌐 API Endpoints

### Authentication
- `POST /api/auth/login` - Seller login

### Offer Management
- `GET /api/offers` - List offers with pagination
- `GET /api/offers/{id}` - Get specific offer
- `POST /api/offers` - Create new offer
- `PUT /api/offers/{id}` - Update offer
- `DELETE /api/offers/{id}` - Delete offer
- `GET /api/offers/seller/{sellerId}` - Get seller's offers

## 🔄 Event Publishing

Publishes RabbitMQ events for search indexing:
- `offer.created` - New offer created
- `offer.updated` - Offer modified
- `offer.deleted` - Offer removed

## 🧪 Testing

### API Testing with cURL
```bash
# Seller login
curl -X POST http://localhost:5002/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "seller@example.com",
    "password": "Password123!"
  }'

# Create offer
curl -X POST http://localhost:5002/api/offers \
  -H "Content-Type: application/json" \
  -d '{
    "vin": "1HGBH41JXMN109186",
    "make": "Honda",
    "model": "Civic",
    "year": 2020,
    "offerAmount": 25000,
    "condition": "Excellent",
    "address": "New York, NY"
  }'
```

## 🐛 Troubleshooting

### Common Issues

1. **Authentication Failures**
   - Verify account-service is running
   - Check user has UserType = 1 (Seller)

2. **RabbitMQ Connection Issues**
   ```bash
   # Check RabbitMQ status
   curl http://localhost:15672/api/overview
   ```

3. **Database Issues**
   ```bash
   dotnet ef migrations add OfferUpdate
   dotnet ef database update
   ```

### Health Check
```bash
curl http://localhost:5002/health
```

## 🔄 Integration

Integrates with:
- **Account Service**: Seller authentication
- **Search Service**: Real-time offer indexing
- **Purchase Service**: Offer availability for buyers
- **Transport Service**: Offer details for transport

## 📈 Monitoring

- Application logs: `docker logs offer-service-api`
- RabbitMQ management: http://localhost:15672
- Database performance monitoring
- Offer creation/update metrics