using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.Providers.Base;

namespace substrate_core.Providers.Registry
{
    public class ServiceProviderRegistry : IServiceProviderRegistry
    {
        private readonly ConcurrentDictionary<Type, object> _services = new();

        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                if (service is T typed) return typed;
                throw new InvalidCastException($"Registered service is not of type {typeof(T).Name}");
            }
            throw new KeyNotFoundException($"Service of type {typeof(T).Name} is not registered.");
        }

        public bool Has<T>() where T : class => _services.ContainsKey(typeof(T));

        public bool TryGet<T>(out T? service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                service = typed;
                return true;
            }
            service = null;
            return false;
        }

        public void Register<T>(T instance) where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            _services[typeof(T)] = instance;
        }

        public IEnumerable<Type> RegisteredTypes => _services.Keys;

        /// <summary>
        /// Resolve a single service of type T. Throws if not found.
        /// </summary>
        public T Resolve<T>() where T : class
        {
            return Get<T>();
        }

        /// <summary>
        /// Resolve all services of type T. Useful if multiple implementations are registered.
        /// </summary>
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            // Return all values that match T
            return _services.Values.OfType<T>();
        }
    }
}