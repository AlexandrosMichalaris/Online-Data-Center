namespace QueueMessageManagement.Interfaces;

public interface IConsumerWithMetadata<T> : IConsumer<T>
{
    string ExchangeName { get; }
    
    string RoutingKey { get; }
    
    bool Durable { get; }
}