using System.Collections.Generic;
using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_shared.types.models
{
    public class VectorBias
    {
        // Core bias state
        public int MoodAxis { get; set; }              // -11 … +11
        public float Persistence { get; set; }         // Multi-sourced, updated per tick
        public ArtifactTilt Tilt { get; set; }         // Enum: Pearl, Crystal, Chaos, RarePearl
        public LegacyTraitLock Legacy { get; set; }    // Enum: None, ResilientHarmony, FracturedLegacy, etc.
        public float DriftMagnitude { get; set; }      // Force per crystallization event
        public float ResonanceScarRatio { get; set; }  // 0 … 1 balance

        // Geometric latent values
        public float Hypotenuse { get; set; }
        public float Area { get; set; }
        public float AngleTheta { get; set; }
        public float SinTheta { get; set; }
        public float CosTheta { get; set; }
        public float TanTheta { get; set; }

        // Functional dials
        public float LogPressure { get; set; }
        public float ExpVolatility { get; set; }

        // Crystallized traits
        public List<Trait> Traits { get; set; } = new();
        public List<TriggerEvent> TriggerEvents { get; set; } = new();
        public IntentType Intent { get; set; }
        public ToneTuple ToneTuple { get; set; }
        public List<Tone> ToneCluster { get; set; } = new();
        public int TickId { get; set; }
    }
}