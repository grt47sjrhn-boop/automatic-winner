using System;
using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_shared.Registries.ResolverRegistry
{
    /// <summary>
    /// Holds a 1-to-1 mapping between descriptor types and their resolvers.
    /// </summary>
    public class DescriptorResolverBootstrapper
    {
        private readonly Dictionary<Type, (BaseDescriptor Descriptor, IFrameResolver Resolver)> _map = new();

        public void Register<TDescriptor>(BaseDescriptor descriptor, IFrameResolver resolver)
            where TDescriptor : BaseDescriptor
        {
            _map[typeof(TDescriptor)] = (descriptor, resolver);
        }

        public bool TryGet<TDescriptor>(out BaseDescriptor descriptor, out IFrameResolver resolver)
            where TDescriptor : BaseDescriptor
        {
            if (_map.TryGetValue(typeof(TDescriptor), out var pair))
            {
                descriptor = pair.Descriptor;
                resolver = pair.Resolver;
                return true;
            }

            descriptor = null!;
            resolver = null!;
            return false;
        }

        public IEnumerable<(BaseDescriptor Descriptor, IFrameResolver Resolver)> GetAll() => _map.Values;
    }
}