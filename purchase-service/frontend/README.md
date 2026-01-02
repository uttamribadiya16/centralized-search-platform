# Purchase Service Frontend

This is the frontend application for the Purchase Service, allowing buyers to browse available offers and make purchases.

## Features

- **Buyer Authentication**: Secure login for buyers only
- **Browse Offers**: View all available vehicle offers from sellers
- **Search & Filter**: Search offers by make, model, year, price, and condition
- **Purchase Requests**: Submit purchase requests for vehicles
- **Purchase Management**: Track and manage your purchase requests
- **Responsive Design**: Works on desktop and mobile devices

## Getting Started

### Prerequisites

- Node.js (version 18 or higher)
- npm or yarn

### Installation

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start the development server:
   ```bash
   npm start
   ```

3. Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

### Environment Variables

Create a `.env` file in the root directory with the following variables:

```
REACT_APP_API_BASE_URL=http://localhost:5003/api
```

## Available Scripts

- `npm start` - Runs the app in development mode
- `npm test` - Launches the test runner
- `npm run build` - Builds the app for production
- `npm run eject` - Ejects from Create React App (irreversible)

## Docker

To run with Docker:

```bash
docker build -t purchase-service-frontend .
docker run -p 3000:80 purchase-service-frontend
```

## Usage

### For Buyers

1. **Login**: Enter your email address to log in as a buyer
2. **Browse Offers**: View all available vehicle offers
3. **Search**: Use filters to find specific vehicles
4. **Purchase**: Click "Purchase" on any available offer to submit a purchase request
5. **Manage Purchases**: View and update the status of your purchases

### Purchase Status Flow

1. **Pending**: Initial purchase request submitted
2. **Confirmed**: Seller has accepted your offer
3. **In Progress**: Payment and paperwork in progress
4. **Completed**: Vehicle successfully purchased
5. **Cancelled**: Purchase was cancelled
6. **Refunded**: Purchase was refunded

## API Integration

The frontend integrates with the Purchase Service API to:

- Authenticate buyers
- Fetch available offers from the Offer Service
- Create and manage purchases
- Publish events for search functionality

## Components

- **LoginPage**: Buyer authentication
- **OfferListPage**: Browse and search available offers
- **PurchaseListPage**: Manage purchase requests
- **PurchaseModal**: Create new purchase requests

## Styling

The application uses custom CSS with a responsive design approach. Key features:

- Clean, professional interface
- Mobile-responsive layout
- Status badges for visual feedback
- Loading states and error handling
- Modal dialogs for actions