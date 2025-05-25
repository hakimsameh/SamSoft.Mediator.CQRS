using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SamSoft.Mediator.CQRS;
using SamSoft.Mediator.CQRS.DefaultBehaviors;
using SamSoft.Mediator.CQRS.ConsoleAppDemo.Command;

Console.WriteLine("Hello, World!");
//var host = Host.CreateDefaultBuilder(args)
//                .ConfigureServices((context, services) =>
//                {
//                    services.AddMediatorService(addPipelineBehavior: true, typeof(MyRequestHandlerExtensions).Assembly);
//                })
//                .ConfigureLogging(logging =>
//                {
//                    logging.ClearProviders();
//                    logging.AddConsole();
//                })
//                .Build();
//await host.RunAsync();



var services = MyRequestHandlerExtensions.CreateServices();

var sender = services.GetService<ISender>();
Console.WriteLine("Write Name:");
var name = Console.ReadLine();
var result = await sender!.Send(new MyQuery(name!));
var encodedName = string.Empty;
var encodedResult = await sender.Send(new EncodeCommand(name!));
if (encodedResult.IsSuccess)
{
    encodedName = encodedResult.Value;
}

if (result.IsSuccess)
{
    Console.WriteLine($"Request handled successfully: {result.Value}");
}
else
{
    Console.WriteLine($"Request failed: {result.Error.Message}");
}
var checkNameResult = await sender.Send(new CheckName(name!, encodedName));
if (checkNameResult.IsSuccess)
{
    Console.WriteLine("Check name succeeded.");
}
else
{
    Console.WriteLine($"Check name failed: {checkNameResult.Error.Message}");
}

public static class MyRequestHandlerExtensions
{
    public static ServiceProvider CreateServices()
    {
        var pipelineBehaviors = new[]
        {
            typeof(AdvancedLoggingBehavior<,>),
            typeof(ValidationBehavior<,>),
            typeof(LoggingPipelineBehavior<,>)
            
        };
        var assemblies = new[] { typeof(MyRequestHandlerExtensions).Assembly };
        var serviceProvider = new ServiceCollection()
            .AddLogging(configure =>
            {
                configure.AddConsole(); // Add Console Logging
                configure.AddDebug();  // Add Debug Logging (optional)
            })

            .AddMediatorCQRS(pipelineBehaviors, assemblies)
            .BuildServiceProvider();
        return serviceProvider;
    }
}
