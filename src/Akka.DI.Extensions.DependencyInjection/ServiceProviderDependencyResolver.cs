using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using Akka.Actor;
using Akka.DI.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Akka.DI.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides services to the <see cref="ActorSystem "/> extension system
    /// used to create actors using the <see cref="Microsoft.Extensions.DependencyInjection"/> container.
    /// </summary>
    public class ServiceProviderDependencyResolver
        : IDependencyResolver, INoSerializationVerificationNeeded
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly ActorSystem _system;

        private readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private readonly ConditionalWeakTable<ActorBase, IServiceScope> _references = new ConditionalWeakTable<ActorBase, IServiceScope>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderDependencyResolver"/> class.
        /// </summary>
        /// <param name="serviceProvider">The container used to resolve references.</param>
        /// <param name="system">The actor system to plug into.</param>
        /// <exception cref="ArgumentNullException">
        /// Either the <paramref name="serviceProvider"/> or the <paramref name="system"/> was null.
        /// </exception>
        public ServiceProviderDependencyResolver(IServiceProvider serviceProvider, ActorSystem system)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (system == null)
            {
                throw new ArgumentNullException(nameof(system));
            }

            _serviceProvider = serviceProvider;
            _system = system;
            system.AddDependencyResolver(this);
        }

        /// <inheritdoc />
        public Type GetType(string actorName)
        {
            _typeCache.TryAdd(
                actorName,
                actorName.GetTypeValue());

            return _typeCache[actorName];
        }

        /// <inheritdoc />
        public Func<ActorBase> CreateActorFactory(Type actorType)
            => () =>
            {
                var scope = _serviceProvider.CreateScope();
                var actor = (ActorBase)scope.ServiceProvider.GetRequiredService(actorType);
                _references.Add(actor, scope);
                return actor;
            };

        /// <inheritdoc />
        public Props Create<TActor>()
            where TActor : ActorBase
            => Create(typeof(TActor));

        /// <inheritdoc />
        public Props Create(Type actorType)
            => _system.GetExtension<DIExt>().Props(actorType);

        /// <inheritdoc />
        public void Release(ActorBase actor)
        {
            if (_references.TryGetValue(actor, out var scope))
            {
                scope.Dispose();
                _references.Remove(actor);
            }
        }
    }
}
