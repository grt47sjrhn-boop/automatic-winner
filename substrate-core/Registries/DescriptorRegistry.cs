using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Registries.Contract.Base;
using substrate_shared.Registries.Manager;

namespace substrate_core.Registries
{
    public class DescriptorRegistry : IEnumerableRegistry<BaseDescriptor>, IRegistryMarker
    {
        public string Name { get; set; }

        private readonly Dictionary<string, BaseDescriptor> _descriptors = new();

        public DescriptorRegistry(string name)
        {
            Name = name;
        }

        public void Register(string key, BaseDescriptor descriptor)
        {
            _descriptors[key] = descriptor;
        }

        public bool TryGet(string key, out BaseDescriptor? descriptor)
        {
            return _descriptors.TryGetValue(key, out descriptor);
        }

        public bool Contains(string key) => _descriptors.ContainsKey(key);

        public void Clear() => _descriptors.Clear();

        public IEnumerable<BaseDescriptor> GetAll() => _descriptors.Values;
    }
}