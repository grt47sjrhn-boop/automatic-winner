using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.models.Profiles;
using substrate_shared.types.Summaries;

namespace substrate_shared.types.structs
{
    /// <summary>
    /// Composite result returned by the resolver pipeline:
    /// - Updated VectorBias
    /// - Outputs from each resolver (delta, tone, persistence, volatility, triggers, personality, intent)
    /// </summary>
    public struct ResolutionResult(VectorBias bias)
    {
        // Always present
        public VectorBias Bias { get; set; } = bias;

        // DeltaVectorResolver
        public DeltaSummary? DeltaSummary { get; set; } = null;

        // ToneClusterResolver
        public ToneResolutionResult? ToneCluster { get; set; } = null;

        // PersistenceResolver
        public PersistenceState? Persistence { get; set; } = null;

        // VolatilityResolver
        public float Volatility { get; set; } = 0f;

        // TriggerResolver
        public List<TriggerEvent> TriggerEvents { get; set; } = [];

        // PersonalityResolver
        public PersonalityState PersonalityState { get; set; } = PersonalityState.Neutral;
        public HardenedBiasType HardenedBias { get; set; } = HardenedBiasType.None;
        public PersonalityProfile? PersonalityProfile { get; set; } = null;

        // IntentActionResolver
        public IntentType IntentBias { get; set; } = default;
        public List<string>? TraceLog { get; set; }
        public ISummary? Summary { get; set; }

        // Constructor for minimal init
    }
}