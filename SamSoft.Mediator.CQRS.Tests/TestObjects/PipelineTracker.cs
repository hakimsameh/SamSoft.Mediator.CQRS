namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

// Tracker for pipeline behavior calls
public static class PipelineTracker
{
    public static bool ValidationWasCalled;
    public static bool LoggingWasCalled;
}
