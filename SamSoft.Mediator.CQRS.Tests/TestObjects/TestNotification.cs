using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;
public record TestNotification(string Message) : INotification;

public class TestNotificationHandlerA : INotificationHandler<TestNotification>
{
    public static List<string> Received = [];
    public Task Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Received.Add("A:" + notification.Message);
        return Task.CompletedTask;
    }
}

public class TestNotificationHandlerB : INotificationHandler<TestNotification>
{
    public static List<string> Received = new();
    public Task Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Received.Add("B:" + notification.Message);
        return Task.CompletedTask;
    }
}
