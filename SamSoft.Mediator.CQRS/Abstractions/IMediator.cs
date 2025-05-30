namespace SamSoft.Mediator.CQRS.Abstractions;

/// <summary>
/// Central mediator interface for sending commands, queries, and publishing notifications.
/// Inherits from <see cref="ISender"/> and <see cref="IPublisher"/>.
/// </summary>
public interface IMediator : ISender, IPublisher
{
}