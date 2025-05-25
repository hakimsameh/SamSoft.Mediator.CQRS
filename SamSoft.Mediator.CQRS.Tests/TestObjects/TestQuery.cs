using SamSoft.Common.Results;
using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

public record TestQuery(string Value) : IQuery<string>;


public class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public Task<Result<string>> Handle(TestQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(query.Value + "_queried"));
}
