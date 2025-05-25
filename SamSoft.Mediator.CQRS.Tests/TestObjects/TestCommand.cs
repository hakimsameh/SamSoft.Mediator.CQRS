using SamSoft.Common.Results;
using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

// Sample command, query, notification, and handlers for testing
public record TestCommand(string Value) : ICommand<string>;

public class TestCommandHandler : ICommandHandler<TestCommand, string>
{
    public Task<Result<string>> Handle(TestCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(command.Value + "_handled"));
}
