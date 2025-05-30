﻿using Microsoft.Extensions.DependencyInjection;
using SamSoft.Mediator.CQRS.Abstractions;
using SamSoft.Mediator.CQRS.Tests.TestObjects;

namespace SamSoft.Mediator.CQRS.Tests;

public class MediatorTests : MediatorTestsBase
{
    [Fact]
    public async Task CommandHandler_ReturnsExpectedResult()
    {
        var sp = BuildServices();
        var mediator = sp.GetRequiredService<IMediator>();
        var result = await mediator.Send(new TestCommand("foo"));
        Assert.True(result.IsSuccess);
        Assert.Equal("foo_handled", result.Value);
    }

    [Fact]
    public async Task QueryHandler_ReturnsExpectedResult()
    {
        var sp = BuildServices();
        var mediator = sp.GetRequiredService<IMediator>();
        var result = await mediator.Send(new TestQuery("bar"));
        Assert.True(result.IsSuccess);
        Assert.Equal("bar_queried", result.Value);
    }

    [Fact]
    public async Task PipelineBehaviors_AreInvoked()
    {
        PipelineTracker.ValidationWasCalled = false;
        PipelineTracker.LoggingWasCalled = false;
        var sp = BuildServices();
        var mediator = sp.GetRequiredService<IMediator>();
        await mediator.Send(new TestCommand("foo"));
        Assert.True(PipelineTracker.ValidationWasCalled);
        Assert.True(PipelineTracker.LoggingWasCalled);
    }

    [Fact]
    public async Task NotificationHandlers_AreAllInvoked()
    {
        TestNotificationHandlerA.Received.Clear();
        TestNotificationHandlerB.Received.Clear();
        var sp = BuildServices();
        var mediator = sp.GetRequiredService<IMediator>();
        await mediator.Publish(new TestNotification("notify"));
        Assert.Contains("A:notify", TestNotificationHandlerA.Received);
        Assert.Contains("B:notify", TestNotificationHandlerB.Received);
    }

    //[Fact]
    //public async Task Mediator_LogsError_WhenHandlerThrowsException()
    //{
    //    var services = new ServiceCollection();
    //    var testLogger = new TestMediatorLogger();
    //    services.AddSingleton<IMediatorLogger>(testLogger);
    //    services.AddMediatorService();
    //    services.AddTransient<ICommandHandler<ThrowingTestCommand>, ThrowingTestCommandHandler>();
    //    var provider = services.BuildServiceProvider();
    //    var mediator = provider.GetRequiredService<IMediator>();

    //    var ex = await Assert.ThrowsAsync<TargetInvocationException>(async () =>
    //    {
    //        await mediator.Send(new ThrowingTestCommand());
    //    });
    //    Assert.IsType<InvalidOperationException>(ex.InnerException);
    //}

    //[Fact]
    //public async Task ValidationBehavior_ThrowsCustomValidationException_OnFailure()
    //{
    //    var services = new ServiceCollection();
    //    services.AddTransient(typeof(FluentValidation.IValidator<TestCommand>), sp => new AlwaysFailingTestCommandValidator());
    //    services.AddMediatorCQRS(addDefaultLogging: false);
    //    var provider = services.BuildServiceProvider();
    //    var mediator = provider.GetRequiredService<IMediator>();

    //    var ex = await Assert.ThrowsAsync<System.Reflection.TargetInvocationException>(async () =>
    //    {
    //        await mediator.Send(new TestCommand("fail"));
    //    });
    //    Assert.IsType<SamSoft.Mediator.CQRS.DefaultBehaviors.CustomValidationException>(ex.InnerException);
    //    var validationEx = (SamSoft.Mediator.CQRS.DefaultBehaviors.CustomValidationException)ex.InnerException!;
    //    Assert.NotEmpty(validationEx.Errors);
    //    //Assert.Contains(validationEx.Errors, e => e.ErrorMessage == "Always fails");
    //}

    // You can add more tests for validation/error scenarios as needed
}
