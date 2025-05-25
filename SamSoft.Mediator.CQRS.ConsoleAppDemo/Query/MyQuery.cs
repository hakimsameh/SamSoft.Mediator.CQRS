﻿// See https://aka.ms/new-console-template for more information
using SamSoft.Common.Results;
using SamSoft.Mediator.CQRS.Abstractions;

public record MyQuery(string Name) : IQuery<string>;
internal sealed class MyRequestHandler : IQueryHandler<MyQuery, string>
{
    public Task<Result<string>> Handle(MyQuery request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Hello {request.Name}");
        return Task.FromResult(Result.Success(request.Name));
    }
}