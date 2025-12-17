using System;
using System.Collections.Generic;
using substrate_shared.Descriptors.Base;
using substrate_shared.Descriptors.Enums;
using substrate_shared.Registries.Contract.Base;
using substrate_shared.Resolvers.Contract.Interfaces;

namespace substrate_shared.Registries.ResolverRegistry
{
    /// <summary>
    /// Registry that maps descriptor types to their associated descriptor + resolver pair.
    /// </summary>
    public class DescriptorResolverRegistry 
        : IEnumerableRegistry<(BaseDescriptor Descriptor, IFrameResolver Resolver)>
    {
        private readonly Dictionary<Type, (BaseDescriptor Descriptor, IFrameResolver Resolver)> _map = new();

        public string Name { get; set; } = nameof(DescriptorResolverRegistry);

        /// <summary>
        /// Register a descriptor + resolver pair keyed by descriptor type.
        /// </summary>
        public void Register(Type key, (BaseDescriptor Descriptor, IFrameResolver Resolver) item)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (item.Descriptor == null) throw new ArgumentNullException(nameof(item.Descriptor));
            if (item.Resolver == null) throw new ArgumentNullException(nameof(item.Resolver));

            _map[key] = item;
        }

        /// <summary>
        /// Convenience overload for strongly typed registration.
        /// </summary>
        public void Register<TDescriptor>(TDescriptor descriptor, IFrameResolver resolver)
            where TDescriptor : BaseDescriptor
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            _map[typeof(TDescriptor)] = (descriptor, resolver);
        }

        /// <summary>
        /// Try to get a descriptor + resolver pair by type key.
        /// </summary>
        public bool TryGet(Type key, out (BaseDescriptor Descriptor, IFrameResolver Resolver) item)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _map.TryGetValue(key, out item);
        }

        /// <summary>
        /// Strongly typed TryGet for a specific descriptor type.
        /// </summary>
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

        /// <summary>
        /// Check if a descriptor type is contained in the registry.
        /// </summary>
        public bool Contains(Type key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _map.ContainsKey(key);
        }

        /// <summary>
        /// Remove all entries.
        /// </summary>
        public void Clear() => _map.Clear();

        /// <summary>
        /// Enumerate all registered descriptor + resolver pairs.
        /// </summary>
        public IEnumerable<(BaseDescriptor Descriptor, IFrameResolver Resolver)> GetAll() => _map.Values;

        public (BaseDescriptor descriptor, IFrameResolver resolver) Resolve(DescriptorType dType)
        {
            foreach (var kvp in _map)
            {
                var (descriptor, resolver) = kvp.Value;
                if (descriptor.Type == dType)
                {
                    return (descriptor, resolver);
                }
            }

            throw new KeyNotFoundException(
                $"No descriptor/resolver pair registered for DescriptorType '{dType}'.");
        }
    }
}