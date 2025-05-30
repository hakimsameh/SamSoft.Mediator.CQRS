using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Logging;

public class ConsoleMediatorLogger : IMediatorLogger
{
    public void LogInformation(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }

    public void LogError(string message, Exception ex)
    {
        Console.WriteLine($"[ERROR] {message}: {ex}");
    }
}