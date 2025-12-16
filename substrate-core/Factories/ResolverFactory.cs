using System.Collections.Generic;
using substrate_core.Models.Resolvers;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Managers;
using substrate_shared.interfaces.Overlays; 
using substrate_shared.structs;

namespace substrate_core.Factories
{
    public static class ResolverFactory
    {
        public static IResolver CreateResolver(
            ResolverType type,
            IEnumerable<BiasVector> vectors,
            IBiasManager biasManager,
            IFacetManager facetManager,
            IToneManager toneManager,
            IRarityManager rarityManager,
            IGeometryOverlay geometryOverlay,
            int tick = 1,          // 🔹 default tick is now 1
            int conflictBand = 1)
        {
            switch (type)
            {
                case ResolverType.Simple:
                    var list = new List<BiasVector>(vectors);
                    if (list.Count != 2)
                        throw new System.ArgumentException("SimpleDuelResolver requires exactly two vectors.");

                    return new SimpleDuelResolver(
                        list[0],
                        list[1],
                        biasManager,
                        facetManager,
                        toneManager,
                        rarityManager,
                        geometryOverlay,
                        tick,          // 🔹 pass tick here
                        conflictBand
                    );

                case ResolverType.MultiAxis:
                    return new MultiAxisDuelResolver(
                        vectors,
                        biasManager,
                        facetManager,
                        toneManager,
                        rarityManager,
                        tick,          // 🔹 pass tick here
                        conflictBand
                    );

                default:
                    throw new System.NotSupportedException($"Resolver type {type} not supported.");
            }
        }
    }
}