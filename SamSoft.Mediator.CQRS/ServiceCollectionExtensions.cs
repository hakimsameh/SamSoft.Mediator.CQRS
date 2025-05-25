namespace SamSoft.Mediator.CQRS;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorCQRS(
        this IServiceCollection services,
        IEnumerable<Type>? pipelineBehaviors = null,
        Assembly[]? assemblies = null)
    {
        assemblies ??= new[] { Assembly.GetCallingAssembly() };

        // Register handlers
        RegisterHandlers(services, assemblies);

        // Register pipeline behaviors in order
        if (pipelineBehaviors != null)
        {
            foreach (var behaviorType in pipelineBehaviors)
                services.AddOpenBehavior(behaviorType);
        }

        // Register validators
        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);

        // Register mediator
        services.AddTransient<IMediator, Mediator>();
        services.AddTransient<ISender>(sp => sp.GetRequiredService<IMediator>());
        services.AddTransient<IPublisher>(sp => sp.GetRequiredService<IMediator>());
        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t =>
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    )
                )
            );

        foreach (var handlerType in handlerTypes)
        {
            foreach (var handlerInterface in handlerType.GetInterfaces()
                .Where(i => i.IsGenericType &&
                    (
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    )
                ))
            {
                services.AddTransient(handlerInterface, handlerType);
            }
        }
    }

    /// <summary>
    /// Registers an open generic pipeline behavior using TryAddEnumerable, allowing multiple behaviors.
    /// </summary>
    public static IServiceCollection AddOpenBehavior(this IServiceCollection services, Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (!openBehaviorType.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must be an open generic type definition");
        }

        var implementedGenericInterfaces = openBehaviorType.GetInterfaces()
            .Where(i => i.IsGenericType)
            .Select(i => i.GetGenericTypeDefinition());

        var implementedOpenBehaviorInterfaces = new HashSet<Type>(implementedGenericInterfaces
            .Where(i => i == typeof(IPipelineBehavior<,>)));

        if (implementedOpenBehaviorInterfaces.Count == 0)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must implement {typeof(IPipelineBehavior<,>).FullName}");
        }

        foreach (var openBehaviorInterface in implementedOpenBehaviorInterfaces)
        {
            services.TryAddEnumerable(new ServiceDescriptor(openBehaviorInterface, openBehaviorType, serviceLifetime));
        }
        return services;
    }

    /// <summary>
    /// Registers an open generic pipeline behavior of type TBehavior.
    /// </summary>
    public static IServiceCollection AddPipelineBehavior<TBehavior>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TBehavior : class
    {
        return services.AddOpenBehavior(typeof(TBehavior), serviceLifetime);
    }
}