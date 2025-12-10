using System;
using System.Collections.Generic;
using substrate_shared.Models;
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
        ToneCut Brilliance { get; }
        RarityTier Rarity { get; }

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