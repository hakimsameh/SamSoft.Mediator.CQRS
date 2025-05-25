using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

public class TestNotificationHandlerA : INotificationHandler<TestNotification>
{
    public static List<string> Received = new();
    public Task Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Received.Add("A:" + notification.Message);
        return Task.CompletedTask;
    }
}
