using System;
using System.Collections.Generic;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Details;
using substrate_shared.structs;
using substrate_shared.Traits.Base;

namespace substrate_core.Models.Engagements.Base
{
    public abstract class EngagementBase : IEngagement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    
        public Guid? BiasSeedId { get; set; }

        public FacetDistribution Shape { get; protected set; }
        public BiasDescriptor Bias { get; protected set; }
        public IToneCut Brilliance { get; protected set; }
        public IRarityTier Rarity { get; protected set; }

        public virtual void ResolveStep(int ticks = 1)
        {
            for (var i = 0; i < ticks; i++)
            {
                ResolveTick();
                UpdateFacets();
                UpdateBias();
                UpdateBrilliance();
                UpdateRarity();
            }
        }

        protected abstract void ResolveTick();

        public abstract void UpdateFacets();
        public abstract void UpdateBias();
        public abstract void UpdateBrilliance();
        public abstract void UpdateRarity();

        public abstract bool IsComplete { get; }
        public IEnumerable<TraitCrystal> ForgedCrystals { get; }
        public int CumulativeResilience { get; }

        // 🔹 Finalize returns ISummary, polymorph by subclass
        public abstract ISummary Finalize();
    }
}