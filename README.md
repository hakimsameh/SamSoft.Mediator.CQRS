# SamSoft.Mediator.CQRS

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/your-repo)
[![Tests](https://img.shields.io/badge/tests-passing-brightgreen)](https://github.com/your-repo)

A modern, extensible, and high-performance .NET library for implementing the CQRS (Command Query Responsibility Segregation) and Mediator patterns. Supports commands, queries, notifications (publish/subscribe), pipeline behaviors, validation, and more.

---

## ✨ Features

- **CQRS and Mediator patterns** for clean separation of concerns
- **Command, Query, and Notification handlers**
- **Pipeline behaviors** (logging, validation, etc.)
- **FluentValidation integration**
- **Publish/Subscribe notifications** (multiple handlers per event)
- **Centralized, flexible DI registration**
- **Interface segregation** (`ISender`, `IPublisher`, `IMediator`)
- **Unit/integration testable**
- **.NET 9+ ready**

---

## 🚀 Installation

1. **NuGet (coming soon):**
   ```sh
   dotnet add package SamSoft.Mediator.CQRS
   ```
2. **Or reference the project directly in your solution.**

---

## 🛠️ Quick Start

### 1. Register in DI

```csharp
services.AddMediatorCQRS(
    pipelineBehaviors: new[] { typeof(ValidationBehavior<,>), typeof(LoggingPipelineBehavior<,>) },
    assemblies: new[] { typeof(YourHandler).Assembly }
);
```

### 2. Define a Command and Handler

```csharp
public record CreateUserCommand(string Name) : ICommand<string>;

public class CreateUserHandler : ICommandHandler<CreateUserCommand, string>
{
    public Task<Result<string>> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success($"User {command.Name} created"));
}
```

### 3. Define a Query and Handler

```csharp
public record GetUserQuery(string Name) : IQuery<string>;

public class GetUserHandler : IQueryHandler<GetUserQuery, string>
{
    public Task<Result<string>> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success($"User: {query.Name}"));
}
```

### 4. Define a Notification and Handlers

```csharp
public record UserCreatedNotification(string Name) : INotification;

public class SendWelcomeEmail : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken = default)
    {
        // Send email logic
        return Task.CompletedTask;
    }
}

public class LogUserCreation : INotificationHandler<UserCreatedNotification>
{
    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken = default)
    {
        // Log logic
        return Task.CompletedTask;
    }
}
```

### 5. Using the Mediator

```csharp
var sender = serviceProvider.GetRequiredService<ISender>();
var result = await sender.Send(new CreateUserCommand("Alice"));

var publisher = serviceProvider.GetRequiredService<IPublisher>();
await publisher.Publish(new UserCreatedNotification("Alice"));
```

---

## 🧩 Pipeline Behaviors

Pipeline behaviors allow you to add cross-cutting concerns (logging, validation, etc.)

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Before
        var response = await next(cancellationToken);
        // After
        return response;
    }
}
```
Register with:
```csharp
services.AddMediatorCQRS(pipelineBehaviors: new[] { typeof(LoggingBehavior<,>) }, assemblies: ...);
```

---

## ✅ Validation

Integrates with [FluentValidation](https://fluentvalidation.net/):

```csharp
public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
```
Validators are discovered automatically from the provided assemblies.

---

## 🧪 Testing

Run tests with:
```sh
dotnet test
```
See the `SamSoft.Mediator.CQRS.Tests` project for examples.

---

## 📦 Contributing

Contributions are welcome! Please open issues or submit pull requests.

---

## 📄 License

[MIT](LICENSE)

---

## 📬 Contact

For questions or support, contact [your-email@example.com](mailto:your-email@example.com)

---

**Tip:**  
- For API documentation, see XML comments in the source or generate docs with DocFX.
- For advanced usage, see the `SamSoft.Mediator.CQRS.Tests` project. 