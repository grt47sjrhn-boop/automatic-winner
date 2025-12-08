using System;
using substrate_shared.Models;
using substrate_shared.structs;

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

        void ResolveStep(int ticks = 1);   // advance engagement by N ticks, default 1
        void UpdateFacets();
        void UpdateBias();
        void UpdateBrilliance();
        void UpdateRarity();

        bool IsComplete { get; }

        // 🔹 Return an ISummary (EventSummary or subclass) at finalization
        ISummary Finalize();
    }
}