using System;
using substrate_shared.interfaces;
using substrate_shared.Models;
using substrate_shared.structs;

namespace substrate_core.Engagements.Base;

public abstract class EngagementBase : IEngagement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid? BiasSeedId { get; set; }

    public FacetDistribution Shape { get; protected set; }
    public BiasDescriptor Bias { get; protected set; }
    public ToneCut Brilliance { get; protected set; }
    public RarityTier Rarity { get; protected set; }

    public virtual void ResolveStep(int ticks = 1)
    {
        for (int i = 0; i < ticks; i++)
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

    // 🔹 Finalize returns ISummary, polymorph by subclass
    public abstract ISummary Finalize();
}