using SamSoft.Mediator.CQRS.Abstractions.Requests;
using SamSoft.Mediator.CQRS.Pipelines;

namespace SamSoft.Mediator.CQRS;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorService(
        this IServiceCollection services,
        Action<MediatorOptions>? configure = null)
    {
        var options = new MediatorOptions();
        configure?.Invoke(options);

        var assemblies = options.AssembliesToRegister.Count > 0
            ? options.AssembliesToRegister
            : [Assembly.GetCallingAssembly()];

        // Handler registration
        var handlerInterfaces = new[]
        {
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>),
            typeof(IQueryHandler<,>),
            typeof(INotificationHandler<>),
            typeof(IRequestHandlerBase<,>)
        };

        var types = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var type in types)
        {
            foreach (var handlerInterface in type.GetInterfaces()
                .Where(i => i.IsGenericType && handlerInterfaces.Contains(i.GetGenericTypeDefinition())))
            {
                services.AddTransient(handlerInterface, type);
            }
        }

        // Register pipeline behaviors
        foreach (var behavior in options.BehaviorsToRegister)
        {
            services.TryAddEnumerable(behavior);
        }

        // Register pre-processors
        foreach (var pre in options.RequestPreProcessorsToRegister)
        {
            services.TryAddEnumerable(pre);
        }

        // Register post-processors
        foreach (var post in options.RequestPostProcessorsToRegister)
        {
            services.TryAddEnumerable(post);
        }

        // Register timeout settings
        services.Configure<TimeoutSettings>(s =>
        {
            s.Timeout = options.TimeoutSettings.Timeout;
            // Copy other properties as needed
        });

        // Register the mediator itself
        services.Add(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), ServiceLifetime.Scoped));
        services.AddTransient<ISender>(sp => sp.GetRequiredService<IMediator>());
        services.AddTransient<IPublisher>(sp => sp.GetRequiredService<IMediator>());
        return services;
    }


    public static IServiceCollection AddMediatorService(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            assemblies = [Assembly.GetCallingAssembly()];
        services.Configure<TimeoutSettings>(options => { });
        // Handler registration
        var handlerInterfaces = new[]
        {
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>),
            typeof(IQueryHandler<,>),
            typeof(IRequestHandlerBase<,>),
            typeof(INotificationHandler<>)
        };

        var types = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var type in types)
        {
            foreach (var handlerInterface in type.GetInterfaces()
                .Where(i => i.IsGenericType && handlerInterfaces.Contains(i.GetGenericTypeDefinition())))
            {
                services.AddTransient(handlerInterface, type);
            }
        }
        // Always add timeout behavior (after logging, before user behaviors)

        /// Add Optional Pipeline Behaviors
        /// 
        services.AddPipelineBehaviors([typeof(TimeoutBehavior<,>),
            typeof(PrePostProcessorBehavior<,>)]);

        // Always add pre/post processor behavior (after timeout, before user behaviors)


        // Register the mediator itself
        services.AddSingleton<IMediator, Mediator>();
        services.AddTransient<ISender>(sp => sp.GetRequiredService<IMediator>());
        services.AddTransient<IPublisher>(sp => sp.GetRequiredService<IMediator>());
        return services;
    }

    // Helper for open generic pipeline behaviors
    public static IServiceCollection AddPipelineBehavior<TBehavior>(this IServiceCollection services)
        where TBehavior : class
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(TBehavior)));
        return services;
    }
    public static IServiceCollection AddPipelineBehaviors(this IServiceCollection services, params Type[] pipelineBehaviors)
    {
        foreach (var behavior in pipelineBehaviors)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), behavior));
        }
        return services;
    }
}