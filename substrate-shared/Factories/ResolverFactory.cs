using System.Collections.Generic;
using substrate_core.Resolvers;
using substrate_shared.interfaces;
using substrate_shared.Resolvers;
using substrate_shared.structs;

namespace substrate_shared.Factories
{
    public enum ResolverType
    {
        Simple,
        MultiAxis
        // Future: Cluster, Trajectory, etc.
    }

    public static class ResolverFactory
    {
        public static IResolver CreateResolver(
            ResolverType type,
            IEnumerable<BiasVector> vectors,
            int conflictBand = 1)
        {
            switch (type)
            {
                case ResolverType.Simple:
                    // Expect exactly two vectors for SimpleDuelResolver
                    var list = new List<BiasVector>(vectors);
                    return list.Count != 2 ? throw new System.ArgumentException("SimpleDuelResolver requires exactly two vectors.") : 
                        new SimpleDuelResolver(list[0], list[1], conflictBand);

                case ResolverType.MultiAxis:
                    return new MultiAxisDuelResolver(vectors, conflictBand);

                default:
                    throw new System.NotSupportedException($"Resolver type {type} not supported.");
            }
        }
    }
}