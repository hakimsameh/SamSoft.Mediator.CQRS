using Microsoft.Extensions.DependencyInjection;
using SamSoft.Mediator.CQRS.Abstractions;
using SamSoft.Mediator.CQRS.Extensions;
using SamSoft.Mediator.CQRS.Tests.TestObjects;
using Xunit;

namespace SamSoft.Mediator.CQRS.Tests;

/// <summary>
/// Tests for notification publish strategies (Sequential, Parallel, Default).
/// </summary>
/// 

[NotificationPublishStrategy(strategy: NotificationPublishStrategy.Sequential)] // Ensures tests run sequentially
public record SequentialNotification(string Message) : INotification;
[NotificationPublishStrategy(strategy: NotificationPublishStrategy.Parallel)]
public record ParallelNotification(string Message) : INotification;
public record DefaultNotification(string Message) : INotification;

public class StrategyTestNotificationHandlerA : INotificationHandler<SequentialNotification>, INotificationHandler<ParallelNotification>, INotificationHandler<DefaultNotification>
{
    public static List<string> Calls = [];
    public Task Handle(SequentialNotification notification, CancellationToken cancellationToken = default)
    {
        Calls.Add("A:" + notification.Message);
        return Task.CompletedTask;
    }
    public Task Handle(ParallelNotification notification, CancellationToken cancellationToken = default)
    {
        Calls.Add("A:" + notification.Message);
        return Task.CompletedTask;
    }
    public Task Handle(DefaultNotification notification, CancellationToken cancellationToken = default)
    {
        Calls.Add("A:" + notification.Message);
        return Task.CompletedTask;
    }
}
public class StrategyTestNotificationHandlerB : INotificationHandler<SequentialNotification>, INotificationHandler<ParallelNotification>, INotificationHandler<DefaultNotification>
{
    public static List<string> Calls = new();
    public Task Handle(SequentialNotification notification, CancellationToken cancellationToken = default)
    {
        Calls.Add("B:" + notification.Message);
        return Task.CompletedTask;
    }
    public Task Handle(ParallelNotification notification, CancellationToken cancellationToken = default)
    {
        Calls.Add("B:" + notification.Message);
        return Task.CompletedTask;
    }
    public Task Handle(DefaultNotification notification, CancellationToken cancellationToken = default)
    {
        Calls.Add("B:" + notification.Message);
        return Task.CompletedTask;
    }
}
public class NotificationPublishStrategyTests
{
    private static ServiceProvider BuildServices()
    {
        var services = new ServiceCollection();
        //services.AddMediatorCQRS(assemblies: [typeof(NotificationPublishStrategyTests).Assembly], addDefaultLogging: false);
        services.AddMediatorService(assemblies: [typeof(StrategyTestNotificationHandlerA).Assembly]);
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Verifies that in Sequential strategy, handlers are called in order and stop on first exception.
    /// </summary>
    [Fact]
    public async Task SequentialNotification_Handlers_AreCalledInOrder()
    {
        StrategyTestNotificationHandlerA.Calls.Clear();
        StrategyTestNotificationHandlerB.Calls.Clear();

        var sp = BuildServices();
        var mediator = sp.GetRequiredService<IMediator>();
        await mediator.Publish(new SequentialNotification("seq"));

        // In sequential, A is always before B
        Assert.Equal("A:seq", StrategyTestNotificationHandlerA.Calls[0]);
        Assert.Equal("B:seq", StrategyTestNotificationHandlerB.Calls[0]);
    }

    /// <summary>
    /// Verifies that in Parallel strategy, both handlers are called (order not guaranteed).
    /// </summary>
    [Fact]
    public async Task ParallelNotification_Handlers_AreCalled()
    {
        StrategyTestNotificationHandlerA.Calls.Clear();
        StrategyTestNotificationHandlerB.Calls.Clear();

        var sp = BuildServices();
        var mediator = sp.GetRequiredService<IMediator>();
        await mediator.Publish(new ParallelNotification("par"));

        // In parallel, order is not guaranteed, but both are called
        Assert.Contains("A:par", StrategyTestNotificationHandlerA.Calls);
        Assert.Contains("B:par", StrategyTestNotificationHandlerB.Calls);
    }

    /// <summary>
    /// Verifies that the default strategy (parallel) calls both handlers.
    /// </summary>
    [Fact]
    public async Task DefaultNotification_Handlers_AreCalledInParallel()
    {
        StrategyTestNotificationHandlerA.Calls.Clear();
        StrategyTestNotificationHandlerB.Calls.Clear();

        var sp = BuildServices();
        var mediator = sp.GetRequiredService<IMediator>();
        await mediator.Publish(new DefaultNotification("def"));

        // Default is parallel, order is not guaranteed, but both are called
        Assert.Contains("A:def", StrategyTestNotificationHandlerA.Calls);
        Assert.Contains("B:def", StrategyTestNotificationHandlerB.Calls);
    }
}

