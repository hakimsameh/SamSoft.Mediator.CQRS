using Microsoft.Extensions.Logging;
using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.DefaultBehaviors;

/// <summary>
/// Pipeline behavior for logging requests, responses, and exceptions.
/// </summary>
public class AdvancedLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<AdvancedLoggingBehavior<TRequest, TResponse>> _logger;

    public AdvancedLoggingBehavior(ILogger<AdvancedLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, HandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest).Name;
        _logger.LogInformation("Handling request: {RequestType} {@Request}", requestType, request);

        try
        {
            var response = await next(cancellationToken);
            _logger.LogInformation("Handled request: {RequestType} => {@Response}", requestType, response);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception handling request: {RequestType} {@Request}", requestType, request);
            throw;
        }
    }
}