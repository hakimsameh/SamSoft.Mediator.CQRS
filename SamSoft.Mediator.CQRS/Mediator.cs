namespace SamSoft.Mediator.CQRS;
using System.Collections.Concurrent;
using System.Linq.Expressions;

/// <summary>
/// Default implementation of the <see cref="IMediator"/> interface for CQRS and Mediator patterns.
/// </summary>
public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private Task<TResponse> ExecutePipeline<TRequest, TResponse>(
        TRequest request,
        Func<Task<TResponse>> handlerInvoker,
        CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "Request cannot be null");
        var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));
        var behaviors = serviceProvider.GetServices(pipelineType)
            .Cast<object>()
            .Reverse()
            .ToList();

        HandlerDelegate<TResponse> next = (CancellationToken t = default) => handlerInvoker();

        foreach (var behavior in behaviors)
        {
            var current = next;
            next = (CancellationToken t = default) =>
            {
                var method = pipelineType.GetMethod(MediatorDefaults.HandlerMethodName);
                if (method == null)
                    throw new InvalidOperationException($"Handle method not found for {pipelineType.Name}");
                return (Task<TResponse>)method.Invoke(behavior, [request, current, cancellationToken])!;
            };
        }

        return next(cancellationToken);
    }

    /// <summary>
    /// Sends a command that does not return a value.
    /// </summary>
    /// <inheritdoc/>
    public Task<Result> Send(ICommand command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"Handler not found for {command.GetType().Name}");
        var method = handlerType.GetMethod(MediatorDefaults.HandlerMethodName);
        if (method == null)
            throw new InvalidOperationException($"Handle method not found for {handlerType.Name}");
        async Task<Result> handlerInvoker() => await (Task<Result>)method.Invoke(handler, [command, cancellationToken])!;
        return ExecutePipeline(command, handlerInvoker, cancellationToken);
    }

    /// <summary>
    /// Sends a command that returns a value.
    /// </summary>
    /// <inheritdoc/>
    public Task<Result<TResponse>> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"Handler not found for {command.GetType().Name}");
        var method = handlerType.GetMethod(MediatorDefaults.HandlerMethodName);
        if (method == null)
            throw new InvalidOperationException($"Handle method not found for {handlerType.Name}");
        async Task<Result<TResponse>> handlerInvoker() => await (Task<Result<TResponse>>)method.Invoke(handler, [command, cancellationToken])!;
        return ExecutePipeline(command, handlerInvoker, cancellationToken);
    }

    /// <summary>
    /// Sends a query that returns a value.
    /// </summary>
    /// <inheritdoc/>
    public Task<Result<TResponse>> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"Handler not found for {query.GetType().Name}");
        var method = handlerType.GetMethod(MediatorDefaults.HandlerMethodName);
        if (method == null)
            throw new InvalidOperationException($"Handle method not found for {handlerType.Name}");

        async Task<Result<TResponse>> handlerInvoker() => await (Task<Result<TResponse>>)method.Invoke(handler, [query, cancellationToken])!;
        return ExecutePipeline(query, handlerInvoker, cancellationToken);
    }

    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    /// <inheritdoc/>
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(typeof(TNotification));
        var handlers = serviceProvider.GetServices(handlerType);

        var tasks = new List<Task>();
        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("Handle");
            if (method == null)
                throw new InvalidOperationException($"Handle method not found for {handlerType.Name}");
            var task = (Task)method.Invoke(handler, new object[] { notification!, cancellationToken })!;
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }
}
