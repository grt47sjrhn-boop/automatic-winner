using System;
using System.Collections.Generic;
using substrate_shared.Providers.Base;

namespace substrate_core.Providers.Registry
{
    public class ServiceProviderRegistry : IServiceProviderRegistry
    {
        private readonly Dictionary<Type, object> _services = new();

        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return service as T ?? throw new InvalidCastException($"Registered service is not of type {typeof(T).Name}");

            throw new KeyNotFoundException($"Service of type {typeof(T).Name} is not registered.");
        }

        public bool Has<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        public void Register<T>(T instance) where T : class
        {
            _services[typeof(T)] = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }
}