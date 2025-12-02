// substrate_core/Resolvers/VolatilityResolver.cs
using System;
using System.Linq;
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
    /// - Accumulate by stretch (hypotenuse)
    /// - Bleed by persistence
    /// - Release on triggers (crystallization/fragmentation)
    /// - Map to bounded scale via logistic
    /// Produces ResolutionResult with DeltaSummary for consistency.
    /// </summary>
    public class VolatilityResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            // Tunable coefficients (safe defaults)
            const float stretchGain   = 0.8f;   // tension added per unit hypotenuse
            const float restDecay     = 0.05f;  // decay when near rest
            const float bleedFactor   = 0.6f;   // persistence bleed per tick
            const float eventRelease  = 0.0003f; // release scaled by event score
            const float restThreshold = 1.0f;   // hypotenuse considered "rest"

            // 1) Accumulate stretch
            var hyp = DebugOverlay.SafeFloat(vb.Hypotenuse);
            vb.AccumulatedTension = DebugOverlay.SafeFloat(vb.AccumulatedTension + hyp * stretchGain);

            // 2) Slow decay if near rest
            if (hyp < restThreshold)
                vb.AccumulatedTension = DebugOverlay.SafeFloat(vb.AccumulatedTension * (1f - restDecay));

            // 3) Bleed tension by persistence (subtract each tick)
            var bleed = DebugOverlay.SafeFloat(vb.Persistence * bleedFactor);
            vb.AccumulatedTension = DebugOverlay.SafeFloat(vb.AccumulatedTension - bleed);

            // 4) Event-based release (crystallization/fragmentation)
            var tickReleaseScore = 0f;
            foreach (var e in vb.TriggerEvents)
            {
                if (e.Type == TriggerType.CrystallizationAttempt)
                    tickReleaseScore += DebugOverlay.SafeFloat(e.Score);
                else if (e.Type == TriggerType.FragmentationAttempt)
                    tickReleaseScore += DebugOverlay.SafeFloat(MathF.Abs(e.Score));
            }
            vb.AccumulatedTension = DebugOverlay.SafeFloat(vb.AccumulatedTension - eventRelease * tickReleaseScore);

            // 5) Clamp tension to narratable range
            vb.AccumulatedTension = DebugOverlay.Clamp(vb.AccumulatedTension, 0f, 1000f);

            // 6) Map accumulated tension to bounded volatility via logistic
            const float k = 0.02f;   // steepness
            const float cap = 100f;  // max contributor-facing volatility
            vb.Volatility = DebugOverlay.SafeFloat(cap / (1f + MathF.Exp(-k * (vb.AccumulatedTension - 200f))));

            DebugOverlay.LogResolver(nameof(VolatilityResolver), vb);

            var summary = new DeltaSummary
            {
                DeltaAxis   = mv.MoodAxis - vb.MoodAxis,
                Magnitude   = mv.MagnitudeFrom(vb.CurrentMood),
                Hypotenuse  = vb.Hypotenuse,
                Area        = vb.Area,
                AngleTheta  = vb.AngleTheta,
                SinTheta    = vb.SinTheta,
                CosTheta    = vb.CosTheta,
                TanTheta    = vb.TanTheta
            };

            return new ResolutionResult(vb, summary);
        }
    }
}