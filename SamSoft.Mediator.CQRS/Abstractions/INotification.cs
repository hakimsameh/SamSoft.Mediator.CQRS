namespace SamSoft.Mediator.CQRS.Abstractions;

/// <summary>
/// Marker interface for notifications (events) that can be published to multiple handlers.
/// </summary>
public interface INotification { }

/// <summary>
/// Handler for a notification (event).
/// </summary>
/// <typeparam name="TNotification">The type of the notification.</typeparam>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles the notification.
    /// </summary>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}