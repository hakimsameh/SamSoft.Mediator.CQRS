using Microsoft.Extensions.DependencyInjection;
using SamSoft.Mediator.CQRS.Pipelines;
using System.Collections.Generic;
using System.Reflection;

namespace SamSoft.Mediator.CQRS;

public class MediatorOptions
{
    public ServiceLifetime Lifetime { get; } = ServiceLifetime.Transient;
    public List<Assembly> AssembliesToRegister { get; } = [];
    public List<ServiceDescriptor> BehaviorsToRegister { get; set; } = [];
    public List<ServiceDescriptor> RequestPreProcessorsToRegister { get; } = [];
    public List<ServiceDescriptor> RequestPostProcessorsToRegister { get; } = [];
    public TimeoutSettings TimeoutSettings { get; } = new TimeoutSettings();
    public void RegisterServicesFromAssemblies(
        params Assembly[] assemblies)
    {
        AssembliesToRegister.AddRange(assemblies);
    }
    public void RegisterServicesFromAssembly(
        Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);
    }
    
}