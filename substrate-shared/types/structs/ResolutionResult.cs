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
    public struct ResolutionResult
    {
        // Always present
        public VectorBias Bias { get; set; }

        // DeltaVectorResolver
        public DeltaSummary DeltaSummary { get; set; }

        // ToneClusterResolver
        public ToneResolutionResult ToneCluster { get; set; }

        // PersistenceResolver
        public PersistenceState Persistence { get; set; }

        // VolatilityResolver
        public float Volatility { get; set; }

        // TriggerResolver
        public List<TriggerEvent> TriggerEvents { get; set; }

        // PersonalityResolver
        public PersonalityState PersonalityState { get; set; }
        public HardenedBiasType HardenedBias { get; set; }
        public PersonalityProfile PersonalityProfile { get; set; }

        // IntentActionResolver
        public IntentType IntentBias { get; set; }
        public List<string> TraceLog { get; set; }
        public ISummary Summary { get; set; }

        // Constructor for minimal init
        public ResolutionResult(VectorBias bias)
        {
            Bias = bias;

            DeltaSummary = default;
            ToneCluster = default;
            Persistence = default;
            Volatility = 0f;
            TriggerEvents = new List<TriggerEvent>();
            PersonalityState = PersonalityState.Neutral;
            HardenedBias = HardenedBiasType.None;
            PersonalityProfile = default;
            IntentBias = default;
        }
    }
}