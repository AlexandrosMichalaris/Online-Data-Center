namespace QueueMessageManagement.Interfaces;

public interface IConsumer<T>
{
    string QueueName { get; }
    
    Task ExecuteAsync(T message, CancellationToken cancellationToken);
}