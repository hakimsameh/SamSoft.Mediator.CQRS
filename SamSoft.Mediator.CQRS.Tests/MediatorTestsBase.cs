using Microsoft.Extensions.DependencyInjection;
using SamSoft.Mediator.CQRS.Tests.TestObjects;

namespace SamSoft.Mediator.CQRS.Tests
{
    public class MediatorTestsBase
    {
        public static ServiceProvider BuildServices()
        {
            var services = new ServiceCollection();
            services.AddMediatorCQRS(
                pipelineBehaviors: [typeof(DummyValidationBehavior<,>), typeof(DummyLoggingBehavior<,>)],
                assemblies: [typeof(MediatorTests).Assembly]
            );
            return services.BuildServiceProvider();
        }
    }
}