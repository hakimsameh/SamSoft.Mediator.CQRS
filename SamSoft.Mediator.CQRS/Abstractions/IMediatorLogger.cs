namespace SamSoft.Mediator.CQRS.Abstractions;

public interface IMediatorLogger
{
    void LogInformation(string message);
    void LogError(string message, Exception ex);
}