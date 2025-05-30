namespace SamSoft.Mediator.CQRS.Handlers;
internal abstract class RequestHandlerBase
{
    public abstract Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
