using System.Collections.Generic;
using substrate_shared.Environment;
using substrate_shared.Facets.Enums;
using substrate_shared.Models;
using substrate_shared.Profiles;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.interfaces
{
    public interface IManager
    {
        // Marker interface for orchestration consistency
    }
    
    // 🔹 Facet manager contract
    public interface IFacetManager : IManager
    {
        FacetDistribution Normalize(IReadOnlyDictionary<FacetType,int> values);
        FacetDistribution Aggregate(IEnumerable<FacetDistribution> distributions);
    }

    // 🔹 Bias manager contract
    public interface IBiasManager : IManager
    {
        BiasDescriptor Summarize(FacetDistribution shape);
        BiasDescriptor Combine(FacetDistribution shape, BiasDescriptor seedBias);
        BiasVector Resolve(BiasVector duelistA, BiasVector duelistB, BiasDescriptor bias);

        // Simple opponent generator
        BiasVector GenerateOpponent();

        // Tuned opponent generator with profile, environment, and optional tilt
        BiasVector GenerateOpponentWeighted(OpponentProfile profile, EnvironmentMood mood, BiasDescriptor? tilt = null);
    }

    // 🔹 Tone manager contract
    public interface IToneManager : IManager
    {
        ToneCut Cut(IReadOnlyDictionary<ToneType,int> toneValues);
        ToneType DetermineDominant(BiasVector speakerA, BiasVector speakerB);
        bool CheckBalance(BiasVector speakerA, BiasVector speakerB);
    }

    // 🔹 Rarity manager contract
    public interface IRarityManager : IManager
    {
        RarityTier AssignTier(int score);
        
        // 🔹 New overload for facet distributions
        int ComputeScore(FacetDistribution distribution);
        
        int ComputeScore(IEngagement engagement);
    }
}