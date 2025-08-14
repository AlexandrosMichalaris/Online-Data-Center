## Overview

This project provides a **modular and type-safe way** to send and receive messages via RabbitMQ.

It is designed to:

- Keep producer and consumer logic separated.

- Allow multiple consumers to run in parallel.

- Use dependency injection (DI) to automatically discover and start consumers.

- Support strongly typed message processing with automatic JSON serialization/deserialization.

---
## Core Components
### 1. `IConsumerBase`

- Non-generic base interface for all consumers.

- Defines:

  - `QueueName` — name of the RabbitMQ queue to listen to.

  - `ExecuteRawAsync(string json, CancellationToken)` — processes raw JSON messages.

- Allows the dispatcher to work with all consumers without knowing their generic type.

---

### 2. `IConsumer<T>`

- Generic interface extending `IConsumerBase`.

- Defines:

  - `ExecuteAsync(T message, CancellationToken)` — type-safe message handling.

---
### 3. `ConsumerBase<T>`

- Abstract class that implements `IConsumer<T>`.

- Handles **JSON deserialization** so you only write business logic in consumers.

- Override `QueueName` and `ExecuteAsync`.

---
### 4. Example Consumer

```csharp

public class TestConsumer : ConsumerBase<string>

{
    public override string QueueName => "test-queue";
    
    public override Task ExecuteAsync(string message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[TestConsumer] Received: {message}");
        return Task.CompletedTask;
    }
}