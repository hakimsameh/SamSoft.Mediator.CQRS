using SamSoft.Common.Results;
using SamSoft.Mediator.CQRS.Abstractions;
using SamSoft.Mediator.CQRS.ConsoleAppDemo.Notifications;


namespace SamSoft.Mediator.CQRS.ConsoleAppDemo.Command;

public record EncodeCommand(string Name) : ICommand<string>;

internal sealed class EncodeCommandHandler : ICommandHandler<EncodeCommand, string>
{
    private readonly IPublisher publisher;

    public EncodeCommandHandler(IPublisher mediator)
    {
        this.publisher = mediator;
    }
    public async Task<Result<string>> Handle(EncodeCommand command, CancellationToken cancellationToken = default)
    {
        // Simulate encoding logic
        var encodedName = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(command.Name));
        var currentDateTime = DateTime.UtcNow;
        Console.WriteLine($"Encoded Name: {encodedName}");
        var notification = new MyNotification($"Hello From Notification Handler {command.Name} On {currentDateTime}");
        await publisher.Publish(notification, cancellationToken);
        return Result.Success(encodedName);
    }
}

public record CheckName(string Name, string Encoded) : ICommand;

internal sealed class CheckNameHandler : ICommandHandler<CheckName>
{
    public Task<Result> Handle(CheckName command, CancellationToken cancellationToken = default)
    {
        // Simulate checking logic
        var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(command.Encoded));
        var isValid = decoded.ToLower().Equals(command.Name.ToLower());
        Console.WriteLine($"Is Name Valid: {isValid}");
        return Task.FromResult(isValid ? 
            Result.Success() : 
            Result.Failure(Error.Validation( "NOtSame","Invalid name or encoded value")));
    }
}