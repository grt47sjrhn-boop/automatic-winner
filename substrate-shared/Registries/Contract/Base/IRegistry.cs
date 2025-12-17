using System;

namespace substrate_shared.Registries.Contract.Base
{
    /// <summary>
    /// Base contract for registries keyed by descriptor type.
    /// </summary>
    public interface IRegistry<T>
    {
        string Name { get; set; }

        /// <summary>
        /// Register an item keyed by its type.
        /// </summary>
        void Register(Type key, T item);

        /// <summary>
        /// Try to get an item by type.
        /// </summary>
        bool TryGet(Type key, out T? item);

        /// <summary>
        /// Check if a type is contained in the registry.
        /// </summary>
        bool Contains(Type key);

        /// <summary>
        /// Remove all entries.
        /// </summary>
        void Clear();
    }
}