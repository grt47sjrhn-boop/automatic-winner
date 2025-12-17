using System;
using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Providers.Contract;

namespace substrate_shared.Registries.StrategyRegistry
{
    public sealed class StrategyRegistry
    {
        private readonly Dictionary<Type, object> _map = new();

        public void Register<T>(IDescriptorStrategy<T> strategy) where T : BaseDescriptor
            => _map[typeof(T)] = strategy;

        public object? Get(Type t) => _map.TryGetValue(t, out var s) ? s : null;

        public void Clear() => _map.Clear();
    }
}