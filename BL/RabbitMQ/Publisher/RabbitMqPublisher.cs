using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BL.RabbitMQ.Publisher;

public class RabbitMqPublisher<T> : IRabbitMqPublisher<T>
{
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly ILogger<RabbitMqPublisher<T>> _logger;
    private readonly IChannel _activeConnectionChannel;

    public RabbitMqPublisher(RabbitMqSettings rabbitMqSettings, RabbitMqConnectionFactory connectionFactory, ILogger<RabbitMqPublisher<T>> logger)
    {
        _rabbitMqSettings = rabbitMqSettings;
        _logger = logger;
        _activeConnectionChannel = connectionFactory.ActiveChannel;
    }

    public async Task Publish(T message, string exchange, string queue)
    {
        try
        {
            await _activeConnectionChannel.BasicPublishAsync(
                exchange: exchange,
                routingKey: queue,
                mandatory: false,
                body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to RabbitMQ. Message: {Message}, Exchange: {Exchange}, Queue: {Queue}",
                JsonSerializer.Serialize(message), exchange, queue);
            throw;
        }
    }
}