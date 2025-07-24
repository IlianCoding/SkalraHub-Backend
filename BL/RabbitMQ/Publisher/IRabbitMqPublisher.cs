namespace BL.RabbitMQ.Publisher;

public interface IRabbitMqPublisher<T>
{
    /// <summary>
    /// This function publishes a message from any given format.
    /// It will publish it on the given exchange and queue, the message can then later on be retrieved.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exchange"></param>
    /// <param name="queue"></param>
    /// <returns></returns>
    Task Publish(T message, string exchange, string queue);
}