namespace SamSoft.Mediator.CQRS.Abstractions;

/// <summary>
/// Interface for processing logic before the main handler is invoked.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IRequestPreProcessor<in TRequest>
{
    Task Process(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Interface for processing logic after the main handler is invoked.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequestPostProcessor<in TRequest, in TResponse>
{
    Task Process(TRequest request, TResponse response, CancellationToken cancellationToken);
}