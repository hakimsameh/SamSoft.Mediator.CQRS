using SamSoft.Common.Results;
using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

public record SlowCommand() : ICommand<string>;

public class SlowCommandHandler : ICommandHandler<SlowCommand, string>
{
    public async Task<Result<string>> Handle(SlowCommand command, CancellationToken cancellationToken = default)
    {
        // Simulate long work and observe cancellation
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        return Result.Success("Done");
    }
}