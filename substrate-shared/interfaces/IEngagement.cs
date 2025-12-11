using System;
using System.Collections.Generic;
using substrate_shared.interfaces.Details;
using substrate_shared.structs;
using substrate_shared.Traits.Base;

namespace substrate_shared.interfaces
{
    public interface IEngagement
    {
        Guid Id { get; }
        Guid? BiasSeedId { get; }

        FacetDistribution Shape { get; }
        BiasDescriptor Bias { get; }
        IToneCut Brilliance { get; }
        IRarityTier Rarity { get; }

        void ResolveStep(int ticks = 1);
        void UpdateFacets();
        void UpdateBias();
        void UpdateBrilliance();
        void UpdateRarity();

        bool IsComplete { get; }
        IEnumerable<TraitCrystal> ForgedCrystals { get; }

        // 🔹 Explicit numeric type
        int CumulativeResilience { get; }

        ISummary Finalize();
    }
}