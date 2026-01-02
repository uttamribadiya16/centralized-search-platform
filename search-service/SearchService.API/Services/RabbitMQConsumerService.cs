using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using SearchService.API.Models;

namespace SearchService.API.Services;

public interface IRabbitMQConsumerService
{
    Task StartConsumingAsync();
    void StopConsuming();
}

public class RabbitMQConsumerService : IRabbitMQConsumerService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<RabbitMQConsumerService> _logger;
    private readonly string _offerQueueName = "offer.events";
    private readonly string _offerExchangeName = "offers";
    private readonly string _purchaseQueueName = "purchase.events";
    private readonly string _purchaseExchangeName = "search_exchange";
    private readonly string _transportQueueName = "transport.events";
    private readonly string _transportExchangeName = "search_exchange";
    private bool _isConsuming = false;

    public RabbitMQConsumerService(
        string connectionString, 
        IElasticsearchService elasticsearchService,
        ILogger<RabbitMQConsumerService> logger)
    {
        _elasticsearchService = elasticsearchService;
        _logger = logger;

        try
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(connectionString);
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare offer exchange
            _channel.ExchangeDeclare(
                exchange: _offerExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            // Declare purchase exchange
            _channel.ExchangeDeclare(
                exchange: _purchaseExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            // Declare offer queue
            _channel.QueueDeclare(
                queue: _offerQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Declare purchase queue
            _channel.QueueDeclare(
                queue: _purchaseQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Declare transport queue
            _channel.QueueDeclare(
                queue: _transportQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Bind offer queue to exchange
            _channel.QueueBind(
                queue: _offerQueueName,
                exchange: _offerExchangeName,
                routingKey: "offer.*" // Listen to offer.created, offer.updated, offer.deleted
            );

            // Bind purchase queue to exchange
            _channel.QueueBind(
                queue: _purchaseQueueName,
                exchange: _purchaseExchangeName,
                routingKey: "purchase.*" // Listen to purchase.created, purchase.updated, purchase.deleted
            );

            // Bind transport queue to exchange
            _channel.QueueBind(
                queue: _transportQueueName,
                exchange: _transportExchangeName,
                routingKey: "transport.*" // Listen to transport.created, transport.updated, transport.deleted
            );

            _logger.LogInformation("RabbitMQ consumer initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ consumer");
            throw;
        }
    }

    public async Task StartConsumingAsync()
    {
        if (_isConsuming)
        {
            _logger.LogWarning("Consumer is already running");
            return;
        }

        try
        {
            // Create consumers for all queues
            var offerConsumer = new EventingBasicConsumer(_channel);
            var purchaseConsumer = new EventingBasicConsumer(_channel);
            var transportConsumer = new EventingBasicConsumer(_channel);

            // Offer consumer
            offerConsumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    _logger.LogInformation("Received offer message with routing key: {RoutingKey}", routingKey);

                    await ProcessMessageAsync(message, routingKey);

                    // Acknowledge the message
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing offer message");
                    
                    // Reject the message and don't requeue to avoid infinite loops
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            // Purchase consumer
            purchaseConsumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    _logger.LogInformation("Received purchase message with routing key: {RoutingKey}", routingKey);

                    await ProcessMessageAsync(message, routingKey);

                    // Acknowledge the message
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing purchase message");
                    
                    // Reject the message and don't requeue to avoid infinite loops
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            // Transport consumer
            transportConsumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    _logger.LogInformation("Received transport message with routing key: {RoutingKey}", routingKey);

                    await ProcessMessageAsync(message, routingKey);

                    // Acknowledge the message
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing transport message");
                    
                    // Reject the message and don't requeue to avoid infinite loops
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            // Start consuming from all queues
            _channel.BasicConsume(
                queue: _offerQueueName,
                autoAck: false,
                consumer: offerConsumer
            );

            _channel.BasicConsume(
                queue: _purchaseQueueName,
                autoAck: false,
                consumer: purchaseConsumer
            );

            _channel.BasicConsume(
                queue: _transportQueueName,
                autoAck: false,
                consumer: transportConsumer
            );

            _isConsuming = true;
            _logger.LogInformation("Started consuming messages from queues: {OfferQueue}, {PurchaseQueue}, {TransportQueue}", 
                _offerQueueName, _purchaseQueueName, _transportQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting message consumption");
            throw;
        }
    }

    private async Task ProcessMessageAsync(string message, string routingKey)
    {
        try
        {
            if (routingKey.StartsWith("offer."))
            {
                await ProcessOfferEventAsync(message, routingKey);
            }
            else if (routingKey.StartsWith("purchase."))
            {
                await ProcessPurchaseEventAsync(message, routingKey);
            }
            else if (routingKey.StartsWith("transport."))
            {
                await ProcessTransportEventAsync(message, routingKey);
            }
            else
            {
                _logger.LogWarning("Unknown routing key pattern: {RoutingKey}", routingKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event with routing key: {RoutingKey}", routingKey);
        }
    }

    private async Task ProcessOfferEventAsync(string message, string routingKey)
    {
        try
        {
            var offerEvent = JsonSerializer.Deserialize<OfferEvent>(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (offerEvent == null)
            {
                _logger.LogWarning("Failed to deserialize offer event");
                return;
            }

            switch (routingKey)
            {
                case "offer.created":
                case "offer.updated":
                    await IndexOfferAsync(offerEvent);
                    break;
                
                case "offer.deleted":
                    await DeleteOfferAsync(offerEvent.Id);
                    break;
                
                default:
                    _logger.LogWarning("Unknown offer routing key: {RoutingKey}", routingKey);
                    break;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize offer event: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing offer event");
        }
    }

    private async Task ProcessPurchaseEventAsync(string message, string routingKey)
    {
        try
        {
            var purchaseEvent = JsonSerializer.Deserialize<PurchaseEvent>(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (purchaseEvent == null)
            {
                _logger.LogWarning("Failed to deserialize purchase event");
                return;
            }

            switch (routingKey)
            {
                case "purchase.created":
                case "purchase.updated":
                    await IndexPurchaseAsync(purchaseEvent);
                    break;
                
                case "purchase.deleted":
                    await DeletePurchaseAsync(purchaseEvent.PurchaseId);
                    break;
                
                default:
                    _logger.LogWarning("Unknown purchase routing key: {RoutingKey}", routingKey);
                    break;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize purchase event: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing purchase event");
        }
    }

    private async Task IndexOfferAsync(OfferEvent offerEvent)
    {
        var offerDocument = new OfferDocument
        {
            Id = offerEvent.Id,
            SellerId = offerEvent.SellerId,
            VIN = offerEvent.VIN,
            Make = offerEvent.Make,
            Model = offerEvent.Model,
            Year = offerEvent.Year,
            OfferAmount = offerEvent.OfferAmount,
            Status = offerEvent.Status,
            Condition = offerEvent.Condition,
            Address = offerEvent.Address,
            CreatedAt = offerEvent.CreatedAt,
            UpdatedAt = offerEvent.UpdatedAt
        };

        var success = await _elasticsearchService.IndexOfferAsync(offerDocument);
        
        if (success)
        {
            _logger.LogInformation("Successfully processed {EventType} event for offer {OfferId}", 
                offerEvent.EventType, offerEvent.Id);
        }
        else
        {
            _logger.LogError("Failed to index offer {OfferId} after {EventType} event", 
                offerEvent.Id, offerEvent.EventType);
        }
    }

    private async Task DeleteOfferAsync(Guid offerId)
    {
        var success = await _elasticsearchService.DeleteOfferAsync(offerId);
        
        if (success)
        {
            _logger.LogInformation("Successfully deleted offer {OfferId} from search index", offerId);
        }
        else
        {
            _logger.LogError("Failed to delete offer {OfferId} from search index", offerId);
        }
    }

    private async Task IndexPurchaseAsync(PurchaseEvent purchaseEvent)
    {
        var purchaseDocument = new PurchaseDocument
        {
            Id = purchaseEvent.PurchaseId,
            BuyerId = purchaseEvent.BuyerId,
            OfferId = purchaseEvent.OfferId,
            SellerId = purchaseEvent.SellerId,
            PurchaseAmount = purchaseEvent.PurchaseAmount,
            Status = purchaseEvent.Status,
            PurchasedAt = purchaseEvent.PurchasedAt,
            Make = purchaseEvent.Make,
            Model = purchaseEvent.Model,
            Year = purchaseEvent.Year
        };

        var success = await _elasticsearchService.IndexPurchaseAsync(purchaseDocument);
        
        if (success)
        {
            _logger.LogInformation("Successfully processed purchase event for purchase {PurchaseId}", 
                purchaseEvent.PurchaseId);
        }
        else
        {
            _logger.LogError("Failed to index purchase {PurchaseId}", purchaseEvent.PurchaseId);
        }
    }

    private async Task DeletePurchaseAsync(Guid purchaseId)
    {
        var success = await _elasticsearchService.DeletePurchaseAsync(purchaseId);
        
        if (success)
        {
            _logger.LogInformation("Successfully deleted purchase {PurchaseId} from search index", purchaseId);
        }
        else
        {
            _logger.LogError("Failed to delete purchase {PurchaseId} from search index", purchaseId);
        }
    }

    private async Task ProcessTransportEventAsync(string message, string routingKey)
    {
        try
        {
            var transportEvent = JsonSerializer.Deserialize<TransportEvent>(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (transportEvent == null)
            {
                _logger.LogWarning("Failed to deserialize transport event");
                return;
            }

            switch (routingKey)
            {
                case "transport.created":
                case "transport.updated":
                case "transport.assigned":
                    await IndexTransportAsync(transportEvent);
                    break;
                
                case "transport.deleted":
                    await DeleteTransportAsync(transportEvent.TransportId);
                    break;
                
                default:
                    _logger.LogWarning("Unknown transport routing key: {RoutingKey}", routingKey);
                    break;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize transport event: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transport event");
        }
    }

    private async Task IndexTransportAsync(TransportEvent transportEvent)
    {
        var transportDocument = new TransportDocument
        {
            Id = transportEvent.TransportId,
            CarrierId = transportEvent.CarrierId,
            PurchaseId = transportEvent.PurchaseId,
            OfferId = transportEvent.OfferId,
            BuyerId = transportEvent.BuyerId,
            SellerId = transportEvent.SellerId,
            Status = transportEvent.Status,
            AssignedAt = transportEvent.AssignedAt,
            UpdatedAt = transportEvent.UpdatedAt,
            TransportFee = transportEvent.TransportFee,
            PickupAddress = transportEvent.PickupAddress,
            DeliveryAddress = transportEvent.DeliveryAddress,
            Vin = transportEvent.VehicleDetails?.Vin,
            Make = transportEvent.VehicleDetails?.Make,
            Model = transportEvent.VehicleDetails?.Model,
            Year = transportEvent.VehicleDetails?.Year
        };

        var success = await _elasticsearchService.IndexTransportAsync(transportDocument);
        
        if (success)
        {
            _logger.LogInformation("Successfully processed transport event for transport {TransportId}", 
                transportEvent.TransportId);
        }
        else
        {
            _logger.LogError("Failed to index transport {TransportId}", transportEvent.TransportId);
        }
    }

    private async Task DeleteTransportAsync(Guid transportId)
    {
        var success = await _elasticsearchService.DeleteTransportAsync(transportId);
        
        if (success)
        {
            _logger.LogInformation("Successfully deleted transport {TransportId} from search index", transportId);
        }
        else
        {
            _logger.LogError("Failed to delete transport {TransportId} from search index", transportId);
        }
    }

    public void StopConsuming()
    {
        if (!_isConsuming)
        {
            return;
        }

        try
        {
            _channel?.Close();
            _connection?.Close();
            _isConsuming = false;
            _logger.LogInformation("Stopped consuming messages");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping message consumption");
        }
    }

    public void Dispose()
    {
        StopConsuming();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

public class RabbitMQHostedService : BackgroundService
{
    private readonly IRabbitMQConsumerService _consumerService;
    private readonly ILogger<RabbitMQHostedService> _logger;

    public RabbitMQHostedService(IRabbitMQConsumerService consumerService, ILogger<RabbitMQHostedService> logger)
    {
        _consumerService = consumerService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ Hosted Service starting...");
        
        // Wait a bit for other services to start
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        
        try
        {
            await _consumerService.StartConsumingAsync();
            
            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RabbitMQ Hosted Service");
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ Hosted Service stopping...");
        _consumerService.StopConsuming();
        await base.StopAsync(stoppingToken);
    }
}