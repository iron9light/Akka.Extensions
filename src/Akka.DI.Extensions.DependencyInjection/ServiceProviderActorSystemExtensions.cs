using System;

using Akka.Actor;
using Akka.DI.Core;

using Microsoft;

namespace Akka.DI.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="ActorSystem"/> to configure <see cref="IServiceProvider"/>.
    /// </summary>
    public static class ServiceProviderActorSystemExtensions
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ServiceProviderDependencyResolver"/> class
        /// associated with the <see cref="ActorSystem"/>.
        /// </summary>
        /// <param name="system">The actor system to plug into.</param>
        /// <param name="serviceProvider">The container used to resolve references.</param>
        /// <returns>The system.</returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="container"/> parameter is null.
        /// </exception>
        public static ActorSystem UseServiceProvider(this ActorSystem system, IServiceProvider serviceProvider)
            => system.UseServiceProvider(serviceProvider, out _);

        /// <summary>
        /// Creates a new instance of the <see cref="ServiceProviderDependencyResolver"/> class
        /// associated with the <see cref="ActorSystem"/>.
        /// </summary>
        /// <param name="system">The actor system to plug into.</param>
        /// <param name="serviceProvider">The container used to resolve references.</param>
        /// <param name="dependencyResolver">The <see cref="ServiceProviderDependencyResolver"/> instance created.</param>
        /// <returns>The system.</returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="container"/> parameter is null.
        /// </exception>
        public static ActorSystem UseServiceProvider(this ActorSystem system, IServiceProvider serviceProvider, out IDependencyResolver dependencyResolver)
        {
            Requires.NotNull(system, nameof(system));
            Requires.NotNull(serviceProvider, nameof(serviceProvider));

            dependencyResolver = new ServiceProviderDependencyResolver(serviceProvider, system);
            return system;
        }
    }
}
