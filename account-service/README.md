# Account Service

User management and authentication service for the Centralized Search Platform.

## 🏗️ Architecture

- **Backend**: .NET Core 8.0 Web API
- **Frontend**: React 18.2.0
- **Database**: SQL Server
- **Port**: Backend (5001), Frontend (3000)

## 📋 Features

- User registration and authentication
- Email/password login
- User type management (Seller, Buyer, Carrier, Agent)
- JWT token authentication
- User profile management

## 🚀 Quick Start with Docker

### Using Docker Compose (Recommended)
```bash
# From project root
docker compose up account-service-backend account-service-frontend sqlserver
```

### Individual Container Setup
```bash
# Start SQL Server
docker run -d --name sqlserver -e ACCEPT_EULA=Y -e SA_PASSWORD=YourStrong@Passw0rd -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest

# Build and run backend
cd account-service/backend
docker build -t account-service-api .
docker run -d -p 5001:5001 --name account-service-api account-service-api

# Build and run frontend
cd ../frontend
docker build -t account-service-web .
docker run -d -p 3000:3000 --name account-service-web account-service-web
```

## 🛠️ Local Development

### Backend Setup
```bash
cd account-service/backend

# Install dependencies
dotnet restore

# Update database
dotnet ef database update

# Run the API
dotnet run
```

### Frontend Setup
```bash
cd account-service/frontend/account-frontend

# Install dependencies
npm install

# Start development server
npm start
```

## 📁 Project Structure

### Backend (`/backend`)
```
AccountService.API/
├── Controllers/
│   └── UsersController.cs      # User management endpoints
├── Data/
│   └── AccountDbContext.cs     # Entity Framework context
├── Models/
│   ├── User.cs                 # User entity
│   └── DTOs/                   # Data transfer objects
├── Services/
│   ├── IUserService.cs         # User service interface
│   └── UserService.cs          # User business logic
├── Migrations/                 # EF Core migrations
└── Program.cs                  # Application startup
```

### Frontend (`/frontend`)
```
account-frontend/
├── src/
│   ├── components/             # React components
│   ├── pages/                  # Page components
│   ├── services/               # API services
│   └── styles/                 # CSS styles
├── public/
└── package.json
```

## 🔧 Configuration

### Backend Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5001
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=account-service;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
```

### Frontend Environment Variables
```bash
REACT_APP_API_URL=http://localhost:5001/api
```

## 📊 Database Schema

### Users Table
| Column | Type | Description |
|--------|------|-------------|
| Id | uniqueidentifier | Primary key |
| FullName | nvarchar(100) | User's full name |
| Email | nvarchar(100) | Email address (unique) |
| Password | nvarchar(255) | Hashed password |
| UserType | int | 1=Seller, 2=Buyer, 3=Carrier, 4=Agent |
| CreatedAt | datetime2 | Account creation timestamp |
| UpdatedAt | datetime2 | Last update timestamp |

## 🌐 API Endpoints

### User Management
- `POST /api/users/register` - Create new user account
- `POST /api/users/login` - User authentication
- `GET /api/users/{id}` - Get user by ID
- `PUT /api/users/{id}` - Update user profile
- `GET /api/users` - List users (admin only)

### Health Check
- `GET /health` - Service health status

## 🧪 Testing

### API Testing with cURL
```bash
# Register new user
curl -X POST http://localhost:5001/api/users/register \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "John Doe",
    "email": "john@example.com",
    "password": "Password123!",
    "userType": 1
  }'

# Login
curl -X POST http://localhost:5001/api/users/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "Password123!"
  }'
```

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Ensure SQL Server is running
   - Check connection string
   - Verify firewall settings

2. **Migration Issues**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **CORS Issues**
   - Check frontend URL in CORS policy
   - Verify API URL in React app

### Health Check
```bash
curl http://localhost:5001/health
```

## 🔄 Integration

This service integrates with:
- **Offer Service**: User authentication for sellers
- **Purchase Service**: User authentication for buyers
- **Transport Service**: User authentication for carriers

## 📈 Monitoring

- Check application logs in Docker: `docker logs account-service-api`
- Monitor database connections
- Track user registration/login metrics

## 🔐 Security

- Passwords are hashed using bcrypt
- Input validation on all endpoints
- SQL injection protection via Entity Framework
- CORS configuration for web security

## Features

- **User Registration**: Create accounts for different user types
- **User Management**: CRUD operations for user accounts
- **Role-Based Access**: Different home pages based on user types
- **Data Persistence**: SQL Server with Entity Framework Core
- **API Documentation**: Swagger/OpenAPI integration
- **Responsive UI**: React-based frontend with modern design

## User Types

1. **Seller** - Users who sell vehicles
   - Manage vehicle offers
   - Track sales performance
   - View analytics

2. **Buyer** - Users who purchase vehicles
   - Browse vehicle inventory
   - Manage watchlists
   - Track purchase history

3. **Carrier** - Users who transport vehicles
   - Manage transport assignments
   - Track deliveries
   - View performance metrics

4. **Agent** - Customer service representatives
   - Access all system data
   - Assist customers
   - Universal search capabilities

## Architecture

### Backend (.NET 8 Web API)
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **API Documentation**: Swagger/OpenAPI
- **Architecture**: Clean Architecture with services and repositories

### Frontend (React)
- **Framework**: React 18
- **Routing**: React Router DOM
- **Styling**: Modern CSS with gradients and animations
- **State Management**: Local state and localStorage
- **HTTP Client**: Axios

## Database Schema

### Users Table
- `Id` (uniqueidentifier) - Primary key
- `FirstName` (nvarchar(100)) - Required
- `LastName` (nvarchar(100)) - Required
- `Email` (nvarchar(255)) - Required, unique
- `PhoneNumber` (nvarchar(20)) - Required
- `UserType` (int) - 1=Seller, 2=Buyer, 3=Carrier, 4=Agent
- `Status` (int) - 1=Active, 2=Inactive, 3=Suspended, 4=Deleted
- `Address` (nvarchar(500)) - Optional
- `City` (nvarchar(100)) - Optional
- `State` (nvarchar(100)) - Optional
- `ZipCode` (nvarchar(20)) - Optional
- `Country` (nvarchar(100)) - Optional
- `CreatedAt` (datetime2) - Auto-generated
- `UpdatedAt` (datetime2) - Auto-updated

## API Endpoints

### Users Controller (`/api/users`)

- `GET /api/users` - Get all users with filtering and pagination
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/by-email/{email}` - Get user by email
- `GET /api/users/by-type/{userType}` - Get users by type
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Soft delete user
- `HEAD /api/users/{id}` - Check if user exists
- `HEAD /api/users/email-exists/{email}` - Check if email exists

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js 16+
- SQL Server or SQL Server Express
- Visual Studio Code or Visual Studio

### Backend Setup
1. Navigate to the backend directory:
   ```bash
   cd account-service/backend/AccountService.API
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=account-service;Integrated Security=True;..."
     }
   }
   ```

4. Run migrations to create database:
   ```bash
   dotnet ef database update
   ```

5. Start the API:
   ```bash
   dotnet run --urls="http://localhost:5001"
   ```

### Frontend Setup
1. Navigate to the frontend directory:
   ```bash
   cd account-service/frontend/account-frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

4. Open browser to `http://localhost:3000`

## Usage

1. **Registration**: Users can register by filling out the signup form and selecting their account type
2. **Dashboard**: After registration, users are redirected to their type-specific dashboard
3. **User Data**: User information is stored in localStorage for session persistence
4. **Logout**: Users can logout using the logout button on their dashboard

## API Testing

The API includes Swagger documentation available at `http://localhost:5001` when running in development mode. You can test all endpoints directly through the Swagger UI.

## Seed Data

The application includes seed data with sample users of each type:
- John Seller (Seller)
- Jane Buyer (Buyer)
- Mike Carrier (Carrier)
- Sarah Agent (Agent)

## Future Enhancements

- JWT authentication
- Password management
- Email verification
- Profile picture upload
- Advanced user search
- Audit logging
- Multi-factor authentication
- Role-based permissions