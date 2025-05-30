using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SamSoft.Common.Results;
using SamSoft.Mediator.CQRS.Abstractions;
using SamSoft.Mediator.CQRS.Extensions;


namespace SamSoft.Mediator.CQRS.Benchmark;



public class MediatorBenchmarks
{
    private Abstractions.IMediator? _samSoftMediator;
    private MediatR.ISender? _mediatRSender;

    [GlobalSetup]
    public void Setup()
    {
        // SamSoft.Mediator.CQRS setup
        var services1 = new ServiceCollection();
        services1.AddMediatorCQRS();
        services1.AddTransient<ICommandHandler<SampleCommand, string>, SampleCommandHandler>();       
        _samSoftMediator = services1.BuildServiceProvider().GetRequiredService<Abstractions.IMediator>();

        // MediatR setup
        var services2 = new ServiceCollection();
        services2.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SampleMediatRHandler>());
        _mediatRSender = services2.BuildServiceProvider().GetRequiredService<MediatR.ISender>();
    }

    [Benchmark]
    public async Task SamSoft_Send_Command()
        => await _samSoftMediator!.Send(new SampleCommand("foo"));

    [Benchmark]
    public async Task MediateR_Send_Command()
        => await _mediatRSender!.Send(new SampleMediatRCommand("foo"));
}
public class SampleCommand(string value) : ICommand<string>
{
    public string Value { get; } = value;
}
public class SampleCommandHandler : ICommandHandler<SampleCommand, string>
{
    public Task<Result<string>> Handle(SampleCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(command.Value + "_handled"));
}

// MediatR command/handler
public record SampleMediatRCommand(string Value) : IRequest<string>;
public class SampleMediatRHandler : MediatR.IRequestHandler<SampleMediatRCommand, string>
{
    public Task<string> Handle(SampleMediatRCommand request, CancellationToken cancellationToken)
        => Task.FromResult(request.Value + "_handled");
}
