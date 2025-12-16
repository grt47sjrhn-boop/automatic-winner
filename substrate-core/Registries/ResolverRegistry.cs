using System.Collections.Generic;
using substrate_shared.Registries.Contract.Base;
using substrate_shared.Registries.Manager;
using substrate_shared.Resolvers.Contract;

namespace substrate_core.Registries
{
    public class ResolverRegistry : IEnumerableRegistry<IFrameResolver>, IRegistryMarker
    {
        public string Name { get; set; }

        private readonly Dictionary<string, IFrameResolver> _resolvers = new();

        public ResolverRegistry(string name)
        {
            Name = name;
        }

        public void Register(string key, IFrameResolver resolver)
        {
            _resolvers[key] = resolver;
        }

        public bool TryGet(string key, out IFrameResolver? resolver)
        {
            return _resolvers.TryGetValue(key, out resolver);
        }

        public bool Contains(string key) => _resolvers.ContainsKey(key);

        public void Clear() => _resolvers.Clear();

        public IEnumerable<IFrameResolver> GetAll() => _resolvers.Values;
    }
}