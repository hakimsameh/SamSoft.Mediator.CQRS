using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

public class TestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
{
    //public static bool WasCalled = false;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        PreAndPostProcessorTracker.PreProcessorWasCalled = true;
        // You can add logic here (e.g., logging, validation, etc.)
        return Task.CompletedTask;
    }
}

public class TestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
{
    //public static bool WasCalled = false;

    public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        PreAndPostProcessorTracker.PostProcessorWasCalled = true;
        // You can add logic here (e.g., auditing, metrics, etc.)
        return Task.CompletedTask;
    }
}