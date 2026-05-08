using System;
using System.Collections.Generic;

namespace StreetFighter.Core
{
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Register<T>(T service) where T : class
        {
            services[typeof(T)] = service;
        }

        public static T Resolve<T>() where T : class
        {
            services.TryGetValue(typeof(T), out var service);
            return service as T;
        }

        public static void RegisterAllServices()
        {
            Register<InputManager>(new InputManager());
            Register<AudioManager>(AudioManager.Instance);
            Register<SaveLoadManager>(new SaveLoadManager());
            Register<GameStateManager>(new GameStateManager());
        }
    }
}
