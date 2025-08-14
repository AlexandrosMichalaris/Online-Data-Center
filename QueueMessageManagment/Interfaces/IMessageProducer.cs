namespace QueueMessageManagement.Interfaces;

public interface IMessageProducer : IAsyncDisposable
{
    Task SendAsync<T>(string queueName, T message, CancellationToken cancellationToken = default);
}