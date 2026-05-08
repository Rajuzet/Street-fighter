using System;
using System.Collections.Generic;

namespace StreetFighter.Core
{
    /// <summary>
    /// Provides a lightweight dependency resolver for runtime services.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        /// <summary>
        /// Registers a service instance for the specified service contract.
        /// </summary>
        /// <typeparam name="T">The service contract.</typeparam>
        /// <param name="service">The instance that implements the contract.</param>
        public static void Register<T>(T service) where T : class
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            Services[typeof(T)] = service;
        }

        /// <summary>
        /// Resolves a registered service instance.
        /// </summary>
        /// <typeparam name="T">The service contract.</typeparam>
        /// <returns>The registered service instance or null if missing.</returns>
        public static T Resolve<T>() where T : class
        {
            Services.TryGetValue(typeof(T), out var service);
            return service as T;
        }

        /// <summary>
        /// Clears all registered services, used when reloading scenes or resetting the game.
        /// </summary>
        public static void Reset()
        {
            Services.Clear();
        }
    }
}
