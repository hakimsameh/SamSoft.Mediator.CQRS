using SamSoft.Mediator.CQRS.Abstractions;
using System;
using System.Collections.Generic;

namespace SamSoft.Mediator.CQRS.Tests.TestObjects;

public class TestMediatorLogger : IMediatorLogger
{
    public List<string> Infos { get; } = new();
    public List<string> Errors { get; } = new();

    public void LogInformation(string message)
    {
        Infos.Add(message);
    }

    public void LogError(string message, Exception ex)
    {
        Errors.Add($"{message}: {ex.Message}");
    }
}