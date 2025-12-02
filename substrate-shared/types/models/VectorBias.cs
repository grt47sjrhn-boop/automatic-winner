using System;
using System.Collections.Generic;
using System.Linq;
using substrate_shared.enums;
using substrate_shared.types.structs;

namespace substrate_shared.types.models
{
    public class VectorBias
    {
        // Core bias state
        public int MoodAxis { get; set; }              // -11 … +11
        public float Persistence { get; set; }         // Updated per tick
        public ArtifactTilt Tilt { get; set; }         // Pearl, Crystal, Chaos, RarePearl
        public LegacyTraitLock Legacy { get; set; }    // None, ResilientHarmony, FracturedLegacy, etc.
        public float DriftMagnitude { get; set; }      // Force per crystallization event
        public float ResonanceScarRatio { get; set; }  // 0 … 1 balance

        // Mood snapshots
        public Mood IncomingMood { get; set; }         // External mood applied this tick
        public Mood CurrentMood { get; set; }          // Snapshot of current bias as a mood

        // Geometric latent values
        public float Hypotenuse { get; set; }
        public float Area { get; set; }
        public float AngleTheta { get; set; }
        public float SinTheta { get; set; }
        public float CosTheta { get; set; }
        public float TanTheta { get; set; }

        // Functional dials
        public float LogPressure { get; set; }
        public float ExpVolatility { get; set; }       // Legacy exponential volatility
        public float AccumulatedTension { get; set; }  // Rubber-band energy
        public float Volatility { get; set; }          // Bounded contributor-facing volatility

        // Traits/events/intents/tones
        public List<Trait> Traits { get; set; } = new();
        public List<TriggerEvent> TriggerEvents { get; set; } = new();
        public IntentType Intent { get; set; }
        public ToneTuple ToneTuple { get; set; }
        public List<Tone> ToneCluster { get; set; } = new();
        public int TickId { get; set; }

        /// <summary>
        /// Baseline persistence calculation:
        /// - Higher when axisDelta is small
        /// - Modified by legacy trait lock and resonance/scar balance
        /// - Allows negative values (erosion states)
        /// </summary>
        public void UpdatePersistence(Mood incoming)
        {
            // Base resistance: how far the incoming axis is from current
            float axisDelta = MathF.Abs(incoming.MoodAxis - MoodAxis);

            // Trait modifiers
            float traitModifier = Legacy == LegacyTraitLock.ResilientHarmony ? 1.2f :
                                  Legacy == LegacyTraitLock.FracturedLegacy ? 0.8f : 1.0f;

            // Resonance/Scar balance centered at 0.5
            float balanceModifier = 1.0f + (ResonanceScarRatio - 0.5f);

            // Derived persistence baseline
            Persistence = (10.0f - axisDelta) * traitModifier * balanceModifier;
        }

        public VectorBias Clone()
        {
            return new VectorBias
            {
                TickId             = this.TickId,
                MoodAxis           = this.MoodAxis,
                Persistence        = this.Persistence,
                DriftMagnitude     = this.DriftMagnitude,
                ResonanceScarRatio = this.ResonanceScarRatio,
                ExpVolatility      = this.ExpVolatility,
                Volatility         = this.Volatility,
                Legacy             = this.Legacy,
                Intent             = this.Intent,
                ToneTuple          = this.ToneTuple, // assuming ToneTuple is immutable (struct)
                
                // Deep copy collections
                Traits        = this.Traits?.Select(t => new Trait
                {
                    Label   = t.Label,
                    Weight = t.Weight
                }).ToList() ?? new List<Trait>(),

                TriggerEvents = this.TriggerEvents?.Select(e => new TriggerEvent
                {
                    Type        = e.Type,
                    Description = e.Description,
                    Magnitude   = e.Magnitude,
                    Score       = e.Score,
                    TickId      = e.TickId,
                    Persistence = e.Persistence,
                    Volatility  = e.Volatility
                }).ToList() ?? new List<TriggerEvent>()
            };
        }

    }
}