using SamSoft.Mediator.CQRS.Abstractions;
using Microsoft.Extensions.Options;

namespace SamSoft.Mediator.CQRS.DefaultBehaviors;

/// <summary>
/// Pipeline behavior that cancels requests if they exceed a specified timeout.
/// Uses Task.WhenAny for robust timeout enforcement.
/// </summary>
public class TimeoutBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly TimeSpan _timeout;

    public TimeoutBehavior(IOptions<TimeoutSettings> options)
    {
        _timeout = options.Value.Timeout;
    }

    public async Task<TResponse> Handle(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var handlerTask = next(cancellationToken);
        var timeoutTask = Task.Delay(_timeout, cancellationToken);

        var completedTask = await Task.WhenAny(handlerTask, timeoutTask);
        if (completedTask == timeoutTask)
            throw new TimeoutException($"Request of type {typeof(TRequest).Name} timed out after {_timeout.TotalMilliseconds} ms.");

        // Await again to propagate exceptions/cancellation from the handler
        return await handlerTask;
    }
}