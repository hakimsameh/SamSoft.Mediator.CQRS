using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SamSoft.Mediator.CQRS.Abstractions;
using SamSoft.Mediator.CQRS.Extensions;
using SamSoft.Mediator.CQRS.Pipelines;

namespace SamSoft.Mediator.CQRS.ConsoleAppDemo;
public static class MyRequestHandlerExtensions
{
    public static ServiceProvider CreateServices()
    {
        //var pipelineBehaviors = new[]
        //{
        //    typeof(AdvancedLoggingBehavior<,>),
        //    typeof(ValidationBehavior<,>),
        //    typeof(LoggingPipelineBehavior<,>)
            
        //};
        var assemblies = new[] { typeof(MyRequestHandlerExtensions).Assembly };
        var serviceProvider = new ServiceCollection()
            .AddLogging(configure =>
            {
                configure.AddConsole(); // Add Console Logging
                configure.AddDebug();  // Add Debug Logging (optional)
            })

            .AddMediatorService(opt=>
            {
                opt.RegisterServicesFromAssemblies(assemblies);
                opt.BehaviorsToRegister.Add(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(AdvancedLoggingBehavior<,>)));
                opt.BehaviorsToRegister.Add(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)));
                opt.BehaviorsToRegister.Add(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>)));
                opt.TimeoutSettings.Timeout = TimeSpan.FromSeconds(10);
            }).BuildServiceProvider();
        return serviceProvider;
    }
}
