using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace BL.RabbitMQ;

public class RabbitMqConnectionFactory
{
    public readonly IChannel ActiveChannel;
    private readonly RabbitMqExchanges _rabbitMqExchanges;
    private readonly RabbitMqQueues _rabbitMqQueues;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;
    private readonly ConnectionFactory _connectionFactory;

    public RabbitMqConnectionFactory(RabbitMqSettings rabbitMqSettings, RabbitMqExchanges rabbitMqExchanges,
        RabbitMqQueues rabbitMqQueues, ILogger<RabbitMqConnectionFactory> logger)
    {
        _rabbitMqExchanges = rabbitMqExchanges;
        _rabbitMqQueues = rabbitMqQueues;
        _logger = logger;
        _connectionFactory = new ConnectionFactory
        {
            HostName = rabbitMqSettings.HostName ?? throw new ArgumentNullException(nameof(rabbitMqSettings.HostName)),
            Port = rabbitMqSettings.Port,
            UserName = rabbitMqSettings.UserName ?? throw new ArgumentNullException(nameof(rabbitMqSettings.UserName)),
            Password = rabbitMqSettings.Password ?? throw new ArgumentNullException(nameof(rabbitMqSettings.Password)),
        };
        ActiveChannel = StartActiveChannel().Result ?? throw new NullReferenceException(nameof(ActiveChannel));
        _ = DeclareAllExchanges();
        _ = DeclareAllQueues();
        _ = BindAllQueues();
    }

    private async Task<IChannel?> StartActiveChannel()
    {
        try
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.CreateChannelAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
    private async Task DeclareAllExchanges()
    {
        await ActiveChannel.ExchangeDeclareAsync(
            exchange: _rabbitMqExchanges.DocumentExchange,
            type: "direct",
            durable: true);
        await ActiveChannel.ExchangeDeclareAsync(
            exchange: _rabbitMqExchanges.StatisticsExchange,
            type: "direct",
            durable: true);
        await ActiveChannel.ExchangeDeclareAsync(
            exchange: _rabbitMqExchanges.UserProfileExchange,
            type: "direct",
            durable: true);
    }
    
    private async Task DeclareAllQueues()
    {
        await ActiveChannel.QueueDeclareAsync(
            queue: _rabbitMqQueues.DocumentUploadQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    private async Task BindAllQueues()
    {
        await ActiveChannel.QueueBindAsync(
            queue: _rabbitMqQueues.DocumentUploadQueue,
            exchange: _rabbitMqExchanges.DocumentExchange,
            routingKey: _rabbitMqQueues.DocumentUploadQueue);
    }
}