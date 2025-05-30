namespace SamSoft.Mediator.CQRS.Handlers.Notifications;
internal interface INotificationPublisher
{
    Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification,
        CancellationToken cancellationToken);
}
internal class ForeachAwaitPublisher : INotificationPublisher
{
    public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
        }
    }
}

internal abstract class NotificationHandlerWrapper
{
    public abstract Task Handle(INotification notification, IServiceProvider serviceProvider, Func<IEnumerable<NotificationHandlerExecutor>, INotification, CancellationToken, Task> publish, CancellationToken cancellationToken);
}

internal class NotificationHandlerWrapperImplementation<TNotification> : NotificationHandlerWrapper where TNotification : INotification
{
    public override Task Handle(INotification notification, IServiceProvider serviceProvider, Func<IEnumerable<NotificationHandlerExecutor>, INotification, CancellationToken, Task> publish, CancellationToken cancellationToken)
    {
        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>()
            .Select(h => new NotificationHandlerExecutor((n, ct) => h.Handle((TNotification)n, ct)));
        return publish(handlers, notification, cancellationToken);
    }
}

public delegate Task HandlerDelegate(CancellationToken cancellationToken = default);

public class NotificationHandlerExecutor(Func<INotification, CancellationToken, Task> handlerCallback)
{
    public Func<INotification, CancellationToken, Task> HandlerCallback { get; } = handlerCallback;
}