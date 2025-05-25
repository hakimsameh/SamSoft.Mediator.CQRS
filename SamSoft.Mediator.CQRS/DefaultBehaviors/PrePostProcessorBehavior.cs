using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.DefaultBehaviors;

/// <summary>
/// Pipeline behavior that runs all registered pre- and post-processors for a request.
/// </summary>
public class PrePostProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IRequestPreProcessor<TRequest>> _preProcessors;
    private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _postProcessors;

    public PrePostProcessorBehavior(
        IEnumerable<IRequestPreProcessor<TRequest>> preProcessors,
        IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors)
    {
        _preProcessors = preProcessors;
        _postProcessors = postProcessors;
    }

    public async Task<TResponse> Handle(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        foreach (var pre in _preProcessors)
            await pre.Process(request, cancellationToken);

        var response = await next(cancellationToken);

        foreach (var post in _postProcessors)
            await post.Process(request, response, cancellationToken);

        return response;
    }
}