# Search Service

Elasticsearch-powered search engine for the Centralized Search Platform.

## 🏗️ Architecture

- **Backend**: .NET Core 8.0 Web API
- **Search Engine**: Elasticsearch 8.11.0
- **Message Broker**: RabbitMQ
- **Port**: API (5003), Elasticsearch (9200)

## 📋 Features

- Full-text search across offers, purchases, and transports
- Real-time indexing via RabbitMQ events
- Role-based search filtering
- Advanced search capabilities
- RESTful search API
- Multi-index search support

## 🚀 Quick Start with Docker

### Using Docker Compose (Recommended)
```bash
# From project root
docker compose up search-service-backend elasticsearch rabbitmq
```

### Individual Container Setup
```bash
# Start Elasticsearch
docker run -d --name elasticsearch \
  -e "discovery.type=single-node" \
  -e "xpack.security.enabled=false" \
  -p 9200:9200 -p 9300:9300 \
  elasticsearch:8.11.0

# Start RabbitMQ
docker run -d --name rabbitmq \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=admin123 \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3.12-management

# Build and run search service
cd search-service/SearchService.API
docker build -t search-service-api .
docker run -d -p 5003:5003 --name search-service-api search-service-api
```

## 🛠️ Local Development

### Backend Setup
```bash
cd search-service/SearchService.API

# Install dependencies
dotnet restore

# Run the API
dotnet run
```

### Prerequisites
- Elasticsearch running on localhost:9200
- RabbitMQ running on localhost:5672

## 📁 Project Structure

```
SearchService.API/
├── Controllers/
│   └── SearchController.cs     # Search API endpoints
├── Models/
│   └── SearchModels.cs         # Search documents and DTOs
├── Services/
│   ├── ElasticsearchService.cs     # Search operations
│   └── RabbitMQConsumerService.cs  # Event consumption
└── Program.cs                  # Application startup
```

## 🔧 Configuration

### Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5003
ConnectionStrings__Elasticsearch=http://localhost:9200
ConnectionStrings__RabbitMQ=amqp://admin:admin123@localhost:5672/
```

## 🗂️ Elasticsearch Indices

### Offers Index
```json
{
  "mappings": {
    "properties": {
      "id": { "type": "keyword" },
      "sellerId": { "type": "keyword" },
      "vin": { "type": "keyword" },
      "make": { "type": "text", "analyzer": "standard" },
      "model": { "type": "text", "analyzer": "standard" },
      "year": { "type": "integer" },
      "offerAmount": { "type": "double" },
      "searchText": { "type": "text", "analyzer": "standard" }
    }
  }
}
```

### Purchases Index
```json
{
  "mappings": {
    "properties": {
      "id": { "type": "keyword" },
      "buyerId": { "type": "keyword" },
      "offerId": { "type": "keyword" },
      "purchaseAmount": { "type": "double" },
      "status": { "type": "keyword" },
      "searchText": { "type": "text" }
    }
  }
}
```

### Transports Index
```json
{
  "mappings": {
    "properties": {
      "id": { "type": "keyword" },
      "carrierId": { "type": "keyword" },
      "purchaseId": { "type": "keyword" },
      "status": { "type": "keyword" },
      "searchText": { "type": "text" }
    }
  }
}
```

## 🌐 API Endpoints

### Search Operations
- `GET /api/search/offers?sellerId={id}&searchText={text}` - Search offers
- `GET /api/search/purchases?buyerId={id}&searchText={text}` - Search purchases  
- `GET /api/search/transports?carrierId={id}&searchText={text}` - Search transports
- `GET /api/search/all?sellerId={id}&buyerId={id}&carrierId={id}&searchText={text}` - Search all

### Health Check
- `GET /api/search/health` - Service health status

## 🔄 Event Consumption

Consumes RabbitMQ events from:

### Offer Events (exchange: "offers")
- `offer.created` - Index new offer
- `offer.updated` - Update offer document
- `offer.deleted` - Remove offer document

### Purchase Events (exchange: "search_exchange")
- `purchase.created` - Index new purchase
- `purchase.updated` - Update purchase document  
- `purchase.deleted` - Remove purchase document

### Transport Events (exchange: "search_exchange")
- `transport.created` - Index new transport
- `transport.updated` - Update transport document
- `transport.deleted` - Remove transport document

## 🧪 Testing

### API Testing with cURL
```bash
# Health check
curl http://localhost:5003/api/search/health

# Search offers
curl "http://localhost:5003/api/search/offers?sellerId=seller-guid&searchText=honda"

# Search purchases
curl "http://localhost:5003/api/search/purchases?buyerId=buyer-guid&searchText=civic"

# Search all
curl "http://localhost:5003/api/search/all?searchText=2020"
```

### Elasticsearch Direct Queries
```bash
# Check cluster health
curl http://localhost:9200/_cluster/health

# List indices
curl http://localhost:9200/_cat/indices?v

# Search offers index
curl -X GET "http://localhost:9200/offers/_search?q=honda"
```

## 🐛 Troubleshooting

### Common Issues

1. **Elasticsearch Connection Failed**
   ```bash
   # Check Elasticsearch status
   curl http://localhost:9200/_cluster/health
   ```

2. **RabbitMQ Connection Issues**
   ```bash
   # Check RabbitMQ management
   curl http://localhost:15672/api/overview
   ```

3. **Index Creation Failed**
   ```bash
   # Manual index creation
   curl -X PUT "http://localhost:9200/offers" -H 'Content-Type: application/json'
   ```

4. **No Search Results**
   - Check if data is indexed
   - Verify event consumption
   - Check index mappings

### Debug Commands
```bash
# View service logs
docker logs search-service-api

# Check consumed messages
docker logs search-service-api | grep "Received.*message"

# Elasticsearch logs
docker logs elasticsearch
```

## 🔄 Integration

Integrates with all platform services:
- **Offer Service**: Real-time offer indexing
- **Purchase Service**: Purchase data indexing
- **Transport Service**: Transport status indexing
- **All Frontend Apps**: Search functionality

## 📈 Monitoring

### Elasticsearch Monitoring
```bash
# Cluster health
curl http://localhost:9200/_cluster/health

# Index statistics
curl http://localhost:9200/_cat/indices?v

# Node information
curl http://localhost:9200/_nodes/stats
```

### Application Monitoring
- Search query performance
- Index update frequency
- RabbitMQ message consumption rate
- Memory usage for large result sets

## 🚀 Performance Tips

1. **Optimize Search Queries**
   - Use specific field searches
   - Implement pagination
   - Use filters instead of queries when possible

2. **Index Optimization**
   - Regular index maintenance
   - Proper field mapping
   - Bulk indexing for large datasets

3. **Monitoring**
   - Track slow queries
   - Monitor index size growth
   - Set up alerting for cluster health