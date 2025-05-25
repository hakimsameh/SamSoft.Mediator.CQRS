using SamSoft.Common.Results;
using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

public class MyQuery : IQuery<string>
{
    public int Id { get; set; }
}
public class MyQueryHandler : IQueryHandler<MyQuery, string>
{
    public Task<Result<string>> Handle(MyQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Id == 0)
            return Task.FromResult(Result.Failure<string>(Error.Validation("InvalidId", "Invalid Id")));
        return Task.FromResult(Result.Success($"Value for {query.Id}"));
    }
}
