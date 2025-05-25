using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.ConsoleAppDemo.Notifications;

public record MyNotification(string NotificationMessage) : INotification;

public class MyNotificationHandler : INotificationHandler<MyNotification>
{
    public Task Handle(MyNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Hello From Notification Handler {notification.NotificationMessage}");
        return Task.CompletedTask;
    }
}
public class MyNotificationHandler2 : INotificationHandler<MyNotification>
{
    public Task Handle(MyNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Hello From Notification Handler 2 {notification.NotificationMessage}");
        return Task.CompletedTask;
    }
}