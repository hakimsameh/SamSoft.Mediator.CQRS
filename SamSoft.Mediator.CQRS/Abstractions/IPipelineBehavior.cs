namespace SamSoft.Mediator.CQRS.Abstractions;

// Pipeline behavior interface
public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(
        TRequest request,
        HandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    );
}

// Delegate for the next handler in the pipeline
public delegate Task<TResponse> HandlerDelegate<TResponse>(CancellationToken t = default);