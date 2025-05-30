using SamSoft.Mediator.CQRS.Abstractions.Requests;

namespace SamSoft.Mediator.CQRS.Abstractions;

// Query with return value
public interface IQuery<TResponse> : IResponseRequest<Result<TResponse>>;

