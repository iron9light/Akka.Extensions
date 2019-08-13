using System;

using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.TestKit;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

namespace Akka.DI.Extensions.DependencyInjection.Tests
{
    public class ServiceProviderDependencyResolverSpec
        : DiResolverSpec
    {
        protected override object NewDiContainer()
        {
            var serviceCollection = new ServiceCollection();
            return serviceCollection;
        }

        protected override IDependencyResolver NewDependencyResolver(object diContainer, ActorSystem system)
        {
            var builder = ToBuilder(diContainer);
            var serviceProvider = builder.BuildServiceProvider();
            var dependencyResolver = new ServiceProviderDependencyResolver(serviceProvider, system);
            return dependencyResolver;
        }

        protected override void Bind<T>(object diContainer, Func<T> generator)
        {
            var builder = ToBuilder(diContainer);
            var t = typeof(T);
            t.IsValueType.Should().BeFalse();
            builder.AddTransient(t, _ => generator());
        }

        protected override void Bind<T>(object diContainer)
        {
            var builder = ToBuilder(diContainer);
            var t = typeof(T);
            t.IsValueType.Should().BeFalse();
            builder.AddTransient(t);
        }

        private static IServiceCollection ToBuilder(object diContainer)
        {
            diContainer.Should().NotBeNull();
            diContainer.Should().BeAssignableTo<IServiceCollection>();
            var builder = diContainer as IServiceCollection;
            builder.Should().NotBeNull();
            return builder!;
        }
    }
}
