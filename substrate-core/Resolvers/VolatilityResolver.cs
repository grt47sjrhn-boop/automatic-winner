// substrate_core/Resolvers/VolatilityResolver.cs
using System;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Computes bounded volatility from accumulated tension:
    /// - Accumulate by stretch (hypotenuse from DeltaSummary)
    /// - Bleed by persistence (PersistenceSummary.Value)
    /// - Optional release on triggers (TriggerSummary.Events)
    /// - Map to bounded scale via logistic
    /// Produces VolatilitySummary for contributor-facing narratability.
    /// </summary>
    public class VolatilityResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            // --- Dependencies ---
            var delta = vb.GetSummary<DeltaSummary>();
            if (delta == null)
                throw new InvalidOperationException("VolatilityResolver requires DeltaSummary.");

            var persistence = vb.GetSummary<PersistenceSummary>();
            if (persistence == null)
                throw new InvalidOperationException("VolatilityResolver requires PersistenceSummary.");

            // Optional: triggers from current/previous tick
            var trigger = vb.GetSummary<TriggerSummary>();

            // --- Tunables (safe defaults) ---
            const float stretchGain   = 0.8f;   // tension added per unit hypotenuse
            const float restDecay     = 0.05f;  // decay when near rest
            const float bleedFactor   = 0.6f;   // persistence bleed per tick
            const float eventReleaseK = 0.0003f; // release scaled by event score
            const float restThreshold = 1.0f;   // hypotenuse considered "rest"

            // --- 1) Accumulate stretch ---
            var hyp = DebugOverlay.SafeFloat(delta.Hypotenuse);
            var accumulatedTension = DebugOverlay.SafeFloat(hyp * stretchGain);

            // --- 2) Slow decay if near rest ---
            if (hyp < restThreshold)
                accumulatedTension = DebugOverlay.SafeFloat(accumulatedTension * (1f - restDecay));

            // --- 3) Bleed tension by persistence ---
            var bleed = DebugOverlay.SafeFloat(persistence.Current * bleedFactor);
            accumulatedTension = DebugOverlay.SafeFloat(accumulatedTension - bleed);

            // --- 4) Event-based release (optional) ---
            var releaseScore = 0f;
            if (trigger is { Events: not null })
            {
                foreach (var e in trigger.Events)
                {
                    if (e.Type == TriggerType.CrystallizationAttempt)
                        releaseScore += DebugOverlay.SafeFloat(e.Score);
                    else if (e.Type == TriggerType.FragmentationAttempt)
                        releaseScore += DebugOverlay.SafeFloat(MathF.Abs(e.Score));
                }
            }
            accumulatedTension = DebugOverlay.SafeFloat(accumulatedTension - eventReleaseK * releaseScore);

            // --- 5) Clamp tension to narratable range ---
            accumulatedTension = DebugOverlay.Clamp(accumulatedTension, 0f, 1000f);

            // --- 6) Map accumulated tension to bounded volatility via logistic ---
            const float k = 0.02f;   // steepness
            const float cap = 100f;  // max contributor-facing volatility
            var volatility = DebugOverlay.SafeFloat(cap / (1f + MathF.Exp(-k * (accumulatedTension - 200f))));

            DebugOverlay.LogResolver(nameof(VolatilityResolver), vb);

            // --- 7) Build and attach volatility summary ---
            var summary = new VolatilitySummary
            {
                TickId             = vb.TickId,

                // Core volatility state
                AccumulatedTension = accumulatedTension,
                Volatility         = volatility,

                // Inputs/modifiers
                PersistenceBleed   = bleed,
                EventReleaseScore  = releaseScore,

                // Geometry context (delta-derived)
                DeltaAxis          = mv.MoodAxis - vb.MoodAxis,
                Magnitude          = mv.MagnitudeFrom(vb.CurrentMood),
                AngleTheta         = delta.AngleTheta
            };

            vb.AddSummary(summary);
            return new ResolutionResult(vb);
        }
    }

    // Consider moving this to substrate_shared.types.Summaries for shared use.
}