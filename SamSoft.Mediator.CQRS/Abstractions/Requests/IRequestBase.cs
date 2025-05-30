namespace SamSoft.Mediator.CQRS.Abstractions.Requests;

public interface IResponseRequest<out TResponse>  { }
public interface IRequestHandlerBase<in TRequest, TResponse>
    where TRequest : IResponseRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
