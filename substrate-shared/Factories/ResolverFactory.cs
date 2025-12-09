using System.Collections.Generic;
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
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            int conflictBand = 1)
        {
            switch (type)
            {
                case ResolverType.Simple:
                    // Expect exactly two vectors for SimpleDuelResolver
                    var list = new List<BiasVector>(vectors);
                    if (list.Count != 2)
                        throw new System.ArgumentException("SimpleDuelResolver requires exactly two vectors.");

                    return new SimpleDuelResolver(
                        list[0],
                        list[1],
                        biasManager,
                        facetManager,
                        toneManager,
                        rarityManager,   // 🔹 now passed in
                        conflictBand
                    );

                case ResolverType.MultiAxis:
                    return new MultiAxisDuelResolver(
                        vectors,
                        biasManager,
                        facetManager,
                        toneManager,
                        rarityManager,
                        conflictBand
                    );

                default:
                    throw new System.NotSupportedException($"Resolver type {type} not supported.");
            }
        }
    }
}