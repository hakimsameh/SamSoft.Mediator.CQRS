using Microsoft.Extensions.DependencyInjection;
using SamSoft.Mediator.CQRS.Abstractions;
using SamSoft.Mediator.CQRS.Extensions;
using SamSoft.Mediator.CQRS.Pipelines;
using SamSoft.Mediator.CQRS.Tests.TestObjects;

namespace SamSoft.Mediator.CQRS.Tests;


public class TimeoutBehaviorTests
{
    private ServiceProvider BuildServices()
    {
        var services = new ServiceCollection();
        services.Configure<TimeoutSettings>(options =>
        {
            options.Timeout = TimeSpan.FromSeconds(1); // 1 second timeout
        });
        //services.AddMediatorCQRS(assemblies: new[] { typeof(SlowCommandHandler).Assembly }, addDefaultLogging: false);
        services.AddMediatorService(assemblies: [typeof(SlowCommandHandler).Assembly]);
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task SlowCommand_Should_ThrowTimeoutException()
    {
        var sp = BuildServices();
        var mediator = sp.GetRequiredService<IMediator>();
        await Assert.ThrowsAsync<TimeoutException>(async () =>
        {
            await mediator.Send(new SlowCommand());
        });
    }
}