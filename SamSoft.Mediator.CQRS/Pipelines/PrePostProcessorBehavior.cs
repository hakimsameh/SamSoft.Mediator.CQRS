namespace SamSoft.Mediator.CQRS.Pipelines;

/// <summary>
/// Pipeline behavior that runs all registered pre- and post-processors for a request.
/// </summary>
public class PrePostProcessorBehavior<TRequest, TResponse>(
    IEnumerable<IRequestPreProcessor<TRequest>> preProcessors,
    IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors) 
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IRequestPreProcessor<TRequest>> _preProcessors = preProcessors;
    private readonly IEnumerable<IRequestPostProcessor<TRequest, TResponse>> _postProcessors = postProcessors;

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