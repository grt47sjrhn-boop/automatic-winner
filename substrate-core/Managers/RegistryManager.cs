using System;
using System.Collections.Generic;
using substrate_shared.Registries.Manager;

namespace substrate_core.Managers
{
    public sealed class RegistryManager : IRegistryManager
    {
        private static readonly Lazy<RegistryManager> _instance = new(() => new RegistryManager());
        public static RegistryManager Instance => _instance.Value;

        private readonly Dictionary<Type, IRegistryMarker> _registries = new();

        private RegistryManager() { }

        public void Register<TRegistry>(TRegistry registry) where TRegistry : class, IRegistryMarker
        {
            _registries[typeof(TRegistry)] = registry;
        }

        public TRegistry Get<TRegistry>() where TRegistry : class, IRegistryMarker
        {
            if (_registries.TryGetValue(typeof(TRegistry), out var registry))
                return registry as TRegistry ?? throw new InvalidCastException();

            throw new KeyNotFoundException($"Registry of type {typeof(TRegistry).Name} not found.");
        }

        public bool Contains<TRegistry>() where TRegistry : class, IRegistryMarker
        {
            return _registries.ContainsKey(typeof(TRegistry));
        }
    }
}