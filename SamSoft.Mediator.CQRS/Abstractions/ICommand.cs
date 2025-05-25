namespace SamSoft.Mediator.CQRS.Abstractions;

// Marker interface for all commands and queries
public interface IBaseCommand { }

// Command without return value
public interface ICommand : IBaseCommand { }

// Command with return value
public interface ICommand<TResponse> : IBaseCommand { }


