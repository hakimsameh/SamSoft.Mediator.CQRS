using SamSoft.Mediator.CQRS.Abstractions;
using Xunit;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;
public record TestNotification(string Message) : INotification;
