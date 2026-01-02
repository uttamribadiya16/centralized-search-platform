using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OfferService.API.Services;

public interface IEventPublisher
{
    Task PublishOfferCreatedAsync(object offerData);
    Task PublishOfferUpdatedAsync(object offerData);
    Task PublishOfferDeletedAsync(object eventData);
}

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly string _exchangeName = "offers";
    private bool _disposed = false;

    public RabbitMQEventPublisher(IConfiguration configuration, ILogger<RabbitMQEventPublisher> logger)
    {
        _logger = logger;
        
        try
        {
            var connectionString = configuration.GetConnectionString("RabbitMQ") ?? "amqp://admin:admin123@localhost:5672/";
            
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

            _logger.LogInformation("RabbitMQ Event Publisher initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ Event Publisher");
            throw;
        }
    }

    public async Task PublishOfferCreatedAsync(object offerData)
    {
        await PublishEventAsync("offer.created", offerData);
    }

    public async Task PublishOfferUpdatedAsync(object offerData)
    {
        await PublishEventAsync("offer.updated", offerData);
    }

    public async Task PublishOfferDeletedAsync(object eventData)
    {
        await PublishEventAsync("offer.deleted", eventData);
    }

    private async Task PublishEventAsync(string routingKey, object eventData)
    {
        try
        {
            if (_disposed)
            {
                _logger.LogWarning("Attempted to publish event on disposed publisher");
                return;
            }

            var message = JsonSerializer.Serialize(eventData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );

            _logger.LogInformation("Published event {RoutingKey} with message ID {MessageId}", 
                routingKey, properties.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {RoutingKey}", routingKey);
            
            // Don't throw here to avoid breaking the main operation
            // In a production system, you might want to implement a retry mechanism
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        try
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            _disposed = true;
            
            _logger.LogInformation("RabbitMQ Event Publisher disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ Event Publisher");
        }
    }
}