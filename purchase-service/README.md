# Purchase Service

Purchase management service for buyers in the Centralized Search Platform.

## 🏗️ Architecture

- **Backend**: .NET Core 8.0 Web API
- **Frontend**: React 18.2.0
- **Database**: SQL Server
- **Message Broker**: RabbitMQ
- **Port**: Backend (5004), Frontend (3002)

## 📋 Features

- Browse available vehicle offers
- Purchase creation and management
- Buyer authentication integration
- Purchase history tracking
- Real-time offer availability
- Search integration via RabbitMQ events

## 🚀 Quick Start with Docker

### Using Docker Compose (Recommended)
```bash
# From project root
docker compose up purchase-service-backend purchase-service-frontend account-service-backend offer-service-backend sqlserver rabbitmq
```

### Individual Container Setup
```bash
# Start dependencies
docker run -d --name sqlserver -e ACCEPT_EULA=Y -e SA_PASSWORD=YourStrong@Passw0rd -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
docker run -d --name rabbitmq -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin123 -p 5672:5672 rabbitmq:3.12-management

# Build and run backend
cd purchase-service/backend
docker build -t purchase-service-api .
docker run -d -p 5004:5004 --name purchase-service-api purchase-service-api

# Build and run frontend
cd ../frontend
docker build -t purchase-service-web .
docker run -d -p 3002:3002 --name purchase-service-web purchase-service-web
```

## 🛠️ Local Development

### Backend Setup
```bash
cd purchase-service/backend

# Install dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the API
dotnet run
```

### Frontend Setup
```bash
cd purchase-service/frontend

# Install dependencies
npm install

# Start development server
npm start
```

## 📁 Project Structure

### Backend (`/backend`)
```
PurchaseService.API/
├── Controllers/
│   ├── AuthController.cs       # Buyer authentication
│   ├── OffersController.cs     # Browse offers
│   └── PurchasesController.cs  # Purchase management
├── Data/
│   └── PurchaseDbContext.cs    # Entity Framework context
├── Models/
│   ├── Purchase.cs             # Purchase entity
│   └── DTOs/                   # Data transfer objects
├── Services/
│   ├── AuthService.cs          # Authentication logic
│   ├── PurchaseService.cs      # Purchase business logic
│   ├── OfferServiceClient.cs   # Offer service integration
│   └── RabbitMQService.cs      # Event publishing
├── Migrations/                 # EF Core migrations
└── Program.cs                  # Application startup
```

### Frontend (`/frontend`)
```
src/
├── components/
│   ├── OfferCard.js           # Offer display
│   ├── PurchaseModal.js       # Purchase creation
│   └── PurchaseHistory.js     # Purchase list
├── pages/
│   ├── LoginPage.js           # Buyer login
│   ├── OfferListPage.js       # Browse offers
│   └── PurchaseListPage.js    # Purchase management
├── services/
│   └── apiService.js          # API integration
└── styles/
```

## 🔧 Configuration

### Backend Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5004
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=purchase-service;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
AccountServiceBaseUrl=http://localhost:5001
OfferServiceBaseUrl=http://localhost:5002
RabbitMQ__HostName=localhost
RabbitMQ__Port=5672
RabbitMQ__UserName=admin
RabbitMQ__Password=admin123
```

### Frontend Environment Variables
```bash
REACT_APP_API_BASE_URL=http://localhost:5004/api
```

## 📊 Database Schema

### Purchases Table
| Column | Type | Description |
|--------|------|-------------|
| Id | uniqueidentifier | Primary key |
| BuyerId | uniqueidentifier | Buyer user ID |
| OfferId | uniqueidentifier | Related offer ID |
| SellerId | uniqueidentifier | Seller user ID |
| PurchaseAmount | decimal(18,2) | Purchase price |
| Status | nvarchar(20) | Purchase status |
| PurchasedAt | datetime2 | Purchase timestamp |
| CreatedAt | datetime2 | Creation timestamp |
| UpdatedAt | datetime2 | Last update timestamp |

## 🌐 API Endpoints

### Authentication
- `POST /api/auth/login` - Buyer login

### Offer Browsing
- `GET /api/offers` - Browse available offers
- `GET /api/offers/{id}` - Get specific offer details

### Purchase Management
- `GET /api/purchases` - List buyer's purchases
- `GET /api/purchases/{id}` - Get specific purchase
- `POST /api/purchases` - Create new purchase
- `PUT /api/purchases/{id}` - Update purchase
- `DELETE /api/purchases/{id}` - Cancel purchase

## 🔄 Event Publishing

Publishes RabbitMQ events:
- `purchase.created` - New purchase created
- `purchase.updated` - Purchase modified
- `purchase.deleted` - Purchase cancelled

## 🧪 Testing

### API Testing with cURL
```bash
# Buyer login
curl -X POST http://localhost:5004/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "buyer@example.com",
    "password": "Password123!"
  }'

# Create purchase
curl -X POST http://localhost:5004/api/purchases \
  -H "Content-Type: application/json" \
  -d '{
    "offerId": "offer-guid-here",
    "purchaseAmount": 25000
  }'
```

## 🐛 Troubleshooting

### Common Issues

1. **Authentication Failures**
   - Verify account-service is running
   - Check user has UserType = 2 (Buyer)

2. **Offer Not Found**
   - Ensure offer-service is running
   - Verify offer exists and is available

3. **Purchase Creation Fails**
   ```bash
   # Check purchase validation
   docker logs purchase-service-api
   ```

### Health Check
```bash
curl http://localhost:5004/health
```

## 🔄 Integration

Integrates with:
- **Account Service**: Buyer authentication
- **Offer Service**: Offer availability and details
- **Search Service**: Purchase indexing
- **Transport Service**: Transport assignment

## 📈 Monitoring

- Application logs: `docker logs purchase-service-api`
- Purchase success/failure rates
- Integration with external services
- RabbitMQ message publishing