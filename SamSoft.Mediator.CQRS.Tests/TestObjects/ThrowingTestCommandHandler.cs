using SamSoft.Mediator.CQRS.Abstractions;
using SamSoft.Common.Results;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

public class ThrowingTestCommand : ICommand
{
    public string Value { get; set; } = "throw";
}

public class ThrowingTestCommandHandler : ICommandHandler<ThrowingTestCommand>
{
    public Task<Result> Handle(ThrowingTestCommand command, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Handler exception");
    }
}