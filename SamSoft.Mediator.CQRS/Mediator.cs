using SamSoft.Mediator.CQRS.Abstractions.Requests;
using SamSoft.Mediator.CQRS.Handlers;
using SamSoft.Mediator.CQRS.Handlers.Notifications;
using SamSoft.Mediator.CQRS.Logging;
using System.Collections.Concurrent;

namespace SamSoft.Mediator.CQRS;


/// <summary>
/// Default implementation of the <see cref="IMediator"/> interface for CQRS and Mediator patterns.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediatorLogger _logger;
    private readonly INotificationPublisher _publisher;

    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> RequestHandlers = new();
    private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> NotificationHandlers = new();
    public Mediator(IServiceProvider serviceProvider, IMediatorLogger? logger = null)
        : this(serviceProvider, new ForeachAwaitPublisher(), logger) { }
    private Mediator(IServiceProvider serviceProvider, INotificationPublisher publisher
        , IMediatorLogger? logger = null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger ?? new ConsoleMediatorLogger();
        _publisher = publisher;
    }

    private Task<TResponse> SendImplementation<TResponse>(IResponseRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = (RequestHandlerWrapper<TResponse>)RequestHandlers.GetOrAdd(request.GetType(), static requestType =>
        {
            var wrapperType = typeof(RequestHandlerWrapperImplementation<,>)
                .MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (RequestHandlerBase)wrapper;
        });

        return handler.Handle(request, _serviceProvider, cancellationToken);
    }
   
    /// <summary>
    /// Sends a command that does not return a value.
    /// </summary>
    /// <inheritdoc/>
    public Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        return SendImplementation(command, cancellationToken);
    }

    /// <summary>
    /// Sends a command that returns a value.
    /// </summary>
    /// <inheritdoc/>
    public Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        return SendImplementation(command, cancellationToken);
    }

    /// <summary>
    /// Sends a query that returns a value.
    /// </summary>
    /// <inheritdoc/>
    public Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        return SendImplementation(query, cancellationToken);
    }
    
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        return PublishNotification(notification, cancellationToken);
    }

    /// <summary>
    /// Override in a derived class to control how the tasks are awaited. By default the implementation calls the <see cref="INotificationPublisher"/>.
    /// </summary>
    /// <param name="handlerExecutors">Enumerable of tasks representing invoking each notification handler</param>
    /// <param name="notification">The notification being published</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing invoking all handlers</returns>
    private Task PublishCore(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        => _publisher.Publish(handlerExecutors, notification, cancellationToken);

    private Task PublishNotification(INotification notification, CancellationToken cancellationToken = default)
    {
        var handler = NotificationHandlers.GetOrAdd(notification.GetType(), static notificationType =>
        {
            var wrapperType = typeof(NotificationHandlerWrapperImplementation<>).MakeGenericType(notificationType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for type {notificationType}");
            return (NotificationHandlerWrapper)wrapper;
        });

        return handler.Handle(notification, _serviceProvider, PublishCore, cancellationToken);
    }
}
