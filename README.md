# SamSoft.Mediator.CQRS

A professional, high-performance, and extensible .NET CQRS (Command Query Responsibility Segregation) mediator library inspired by MediatR, with modern DI integration, pipeline behaviors, and advanced configuration.

---

## 🚀 Features

- **CQRS Abstractions**: Clean interfaces for commands, queries, and notifications.
- **Handler Registration**: Automatic discovery and registration of handlers (internal or public) from assemblies.
- **Pipeline Behaviors**: Pluggable, ordered pipeline for cross-cutting concerns (logging, validation, timeouts, etc.).
- **Pre/Post Processors**: Support for request pre- and post-processing behaviors.
- **Notification/Event Publishing**: Publish events to multiple handlers with custom strategies.
- **Configurable Lifetime**: Choose Singleton, Scoped, or Transient for the mediator.
- **Extensible Configuration**: Register behaviors, processors, and assemblies via a single options object.
- **Optimized Performance**: Delegate pipeline caching for near-zero overhead dispatch (on par with MediatR).
- **Full .NET DI Integration**: Works with Microsoft.Extensions.DependencyInjection.

---

## 🛠️ Quick Start

```csharp
// In your Startup.cs or Program.cs
services.AddMediatorService(options =>
{
    options.Lifetime = ServiceLifetime.Scoped; // or Singleton/Transient
    options.RegisterServicesFromAssembly(typeof(MyHandler).Assembly);
    options.BehaviorsToRegister.Add(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(TimeoutBehavior<,>)));
    options.BehaviorsToRegister.Add(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(PrePostProcessorBehavior<,>)));
    // Add pre/post processors as needed
    options.TimeoutSettings.Timeout = TimeSpan.FromSeconds(10);
});
```

---

## ✨ Example Usage

```csharp
// Define a command
public class CreateUserCommand : ICommand<string> { /* ... */ }

// Implement a handler
internal class CreateUserHandler : ICommandHandler<CreateUserCommand, string>
{
    public Task<Result<string>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // ...
    }
}

// Send a command
var result = await mediator.Send(new CreateUserCommand(...));

// Define and handle a notification
public class UserCreatedNotification : INotification { /* ... */ }
internal class UserCreatedHandler : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken) { /* ... */ }
}
await mediator.Publish(new UserCreatedNotification(...));
```

---

## ⚡ Benchmarks

| Method                | Mean     | Error    | StdDev   | Gen0   | Allocated |
|---------------------- |---------:|---------:|---------:|-------:|----------:|
| SamSoft_Send_Command  | 389.7 ns | 28.04 ns | 82.66 ns | 0.0896 |     376 B |
| MediateR_Send_Command | 423.8 ns | 31.48 ns | 92.83 ns | 0.0801 |     336 B |


- **Performance is on par with MediatR** for command dispatch.
- Pipeline behaviors add minimal overhead (as expected for any mediator).
- Optimized for real-world, production-grade scenarios.

---

## 🧩 Extensibility

- Add custom pipeline behaviors, pre/post processors, and notification publishers.
- Register handlers and behaviors from any assembly.
- Configure all options via `MediatorOptions`.

---

## 📦 Why Choose SamSoft.Mediator.CQRS?

- Professional, modern CQRS mediator for .NET
- MediatR-like API and performance, but fully customizable
- Designed for enterprise, modular, and high-performance applications

---

## 📚 Documentation

- See XML docs and code comments for API details.
- For advanced scenarios, see the `MediatorOptions` class and extension methods.

---

## 🏆 License

MIT

---

## 📬 Contact

For questions or support, contact [hakimsameh70@gmail.com](mailto:hakimsameh70@gmail.com)

---

**Tip:**  
- For API documentation, see XML comments in the source or generate docs with DocFX.
- For advanced usage, see the `SamSoft.Mediator.CQRS.Tests` project.

## Getting Started

1. **Install the NuGet package:**
   ```sh
   dotnet add package SamSoft.Mediator.CQRS
   ```

2. **Register the Mediator in your DI container:**
   ```csharp
   services.AddMediatorCQRS();
   ```

## Defining Requests and Handlers

### Command
```csharp
public class MyCommand(string value) : ICommand<string>
{
    public string Value { get; } = value;
}

public class MyCommandHandler : ICommandHandler<MyCommand, string>
{
    public Task<Result<string>> Handle(MyCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(command.Value + "_handled"));
}
```

### Query
```csharp
public class MyQuery(int id) : IQuery<string>
{
    public int Id { get; } = id;
}

public class MyQueryHandler : IQueryHandler<MyQuery, string>
{
    public Task<Result<string>> Handle(MyQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success($"Value for {query.Id}"));
}
```

### Notification
```csharp
public class MyNotification(string message) : INotification
{
    public string Message { get; } = message;
}

public class MyNotificationHandler : INotificationHandler<MyNotification>
{
    public Task Handle(MyNotification notification, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Received: {notification.Message}");
        return Task.CompletedTask;
    }
}
```

## Using the Mediator
```csharp
var result = await mediator.Send(new MyCommand("foo"));
var queryResult = await mediator.Send(new MyQuery(1));
await mediator.Publish(new MyNotification("Hello!"));
```

## Pipeline Behaviors (Decorators)
You can add cross-cutting concerns (logging, validation, etc.) via pipeline behaviors:

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");
        var response = await next(cancellationToken);
        Console.WriteLine($"Handled {typeof(TRequest).Name}");
        return response;
    }
}
```
Register with:
```csharp
services.AddPipelineBehavior<LoggingBehavior<,>>();
```

## Exception Handling and Logging
- Exceptions in handlers and pipeline behaviors are logged via `IMediatorLogger`.
- You can provide your own logger by implementing `IMediatorLogger` and registering it in DI.

## Validation
- Add validators by implementing `FluentValidation.IValidator<TRequest>`.
- Validators are automatically picked up if registered before `AddMediatorCQRS`.
- Validation failures throw a `CustomValidationException` (see pipeline behavior).

## Testing
- Handlers and pipeline behaviors can be tested in isolation or via integration tests.
- Use a test logger to assert logging behavior.

## Advanced
- Supports notification publish strategies (parallel/sequential).
- Reflection caching for performance.
- Thread-safe and production-ready with extensibility in mind.

---
For more details, see the source code and XML documentation comments. 