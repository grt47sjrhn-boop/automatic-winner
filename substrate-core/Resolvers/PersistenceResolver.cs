using System;
using System.Linq;
using substrate_core.Utilities;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Resolves persistence values based on baseline axis/legacy/resonance rules
    /// plus geometric deltas and trait weights.
    /// Produces updated VectorBias plus a DeltaSummary snapshot for downstream volatility/narration.
    /// </summary>
    public class PersistenceResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood im)
        {
            // 1) Baseline persistence from axis distance, legacy trait lock, resonance/scar balance
            vb.UpdatePersistence(im);

            // 2) Geometric adjustments
            float traitWeightSum = vb.Traits?.Sum(t => DebugOverlay.SafeFloat(t.Weight)) ?? 0f;

            const float areaFactor       = 0.25f;
            const float hypotenuseFactor = 0.35f;
            const float angleFactor      = 0.15f;
            const float traitFactor      = 0.1f;

            float geomAdj =
                (vb.Area * areaFactor) +
                (vb.Hypotenuse * hypotenuseFactor) +
                (1 - Math.Abs(vb.AngleTheta) / (float)Math.PI) * angleFactor +
                (traitWeightSum * traitFactor) -
                vb.LogPressure;

            // 3) Blend baseline + geometry, allow negatives for erosion states
            vb.Persistence = DebugOverlay.Clamp(vb.Persistence + geomAdj, -100f, 100f);

            // 4) Log pressure derived safely
            float logArg = 1 + vb.Persistence;
            vb.LogPressure = (logArg <= 0f) ? 0f : DebugOverlay.SafeFloat(MathF.Log(logArg));

            // 5) Legacy exponential volatility (optional dial, capped)
            float expArg = vb.Persistence * vb.DriftMagnitude;
            vb.ExpVolatility = DebugOverlay.SafeFloat(MathF.Exp(DebugOverlay.Clamp(expArg, -10f, 10f)));

            DebugOverlay.LogResolver(nameof(PersistenceResolver), vb);

            // 6) Build DeltaSummary snapshot for consistency
            var summary = new DeltaSummary
            {
                DeltaAxis   = im.MoodAxis - vb.MoodAxis,
                Magnitude   = im.MagnitudeFrom(vb.CurrentMood),
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