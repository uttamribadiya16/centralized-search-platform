using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PurchaseService.API.Services;

public interface IEventPublisher
{
    Task PublishAsync(string routingKey, object eventData);
}

public class RabbitMQEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly string _exchangeName = "search_exchange";
    private bool _disposed = false;

    public RabbitMQEventPublisher(IConfiguration configuration, ILogger<RabbitMQEventPublisher> logger)
    {
        _logger = logger;
        
        try
        {
            var rabbitConfig = configuration.GetSection("RabbitMQ");
            var hostName = rabbitConfig["HostName"] ?? "localhost";
            var port = int.Parse(rabbitConfig["Port"] ?? "5672");
            var userName = rabbitConfig["UserName"] ?? "guest";
            var password = rabbitConfig["Password"] ?? "guest";
            
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            
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

    public async Task PublishAsync(string routingKey, object eventData)
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

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {RoutingKey}", routingKey);
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                _channel?.Close();
                _channel?.Dispose();
                _connection?.Close();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing RabbitMQ Event Publisher");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}