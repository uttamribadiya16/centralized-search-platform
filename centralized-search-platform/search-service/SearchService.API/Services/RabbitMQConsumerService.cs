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
    private readonly string _queueName = "offer.events";
    private readonly string _exchangeName = "offers";
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

            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            // Declare queue
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Bind queue to exchange
            _channel.QueueBind(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: "offer.*" // Listen to offer.created, offer.updated, offer.deleted
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
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    _logger.LogInformation("Received message with routing key: {RoutingKey}", routingKey);

                    await ProcessMessageAsync(message, routingKey);

                    // Acknowledge the message
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    
                    // Reject the message and don't requeue to avoid infinite loops
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer
            );

            _isConsuming = true;
            _logger.LogInformation("Started consuming messages from queue: {QueueName}", _queueName);
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
                    _logger.LogWarning("Unknown routing key: {RoutingKey}", routingKey);
                    break;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing offer event");
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