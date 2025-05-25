using SamSoft.Mediator.CQRS.Abstractions;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

// Sample command, query, notification, and handlers for testing
public record TestCommand(string Value) : ICommand<string>;
