# Account Service

The Account Service is responsible for managing user accounts across the centralized search platform. It handles user registration, authentication, and user type-based functionality for Sellers, Buyers, Carriers, and Agents.

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