using System;
using substrate_core.Utilities;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries; // DeltaSummary, PersistenceSummary

namespace substrate_core.Resolvers
{
    /// <summary>
    /// PersistenceResolver:
    /// - Queries DeltaSummary from VectorBias
    /// - Calculates directional persistence trajectory
    /// - Applies volatility-driven erosion to adjustments
    /// - Attaches PersistenceSummary for PersonalityProfile to narrate
    /// </summary>
    public class PersistenceResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood incoming)
        {
            // --- 1) Require DeltaSummary ---
            var delta = vb.GetSummary<DeltaSummary>();
            if (delta == null)
                throw new InvalidOperationException("DeltaSummary must be resolved before PersistenceResolver.");

            // --- 2) Baseline + previous pressure ---
            var prevPersistence = vb.GetSummary<PersistenceSummary>();
            var baseline        = prevPersistence?.Current ?? 0f;
            var prevLogPressure = prevPersistence?.LogPressure ?? 0f;

            // --- 3) Geometric adjustment (unsigned magnitude) ---
            const float areaFactor       = 0.25f;
            const float hypotenuseFactor = 0.35f;
            const float angleFactor      = 0.15f;

            var geomAdj =
                (delta.Area * areaFactor) +
                (delta.Hypotenuse * hypotenuseFactor) +
                (1 - Math.Abs(delta.AngleTheta) / (float)Math.PI) * angleFactor -
                prevLogPressure;

            // --- 4) Direction from geometry ---
            // Prefer axis delta; fall back to angle sign if axis is ~0
            var axisDelta   = DebugOverlay.SafeFloat(delta.DeltaAxis);
            var direction   = MathF.Abs(axisDelta) > 1e-3f ? MathF.Sign(axisDelta) : MathF.Sign(delta.AngleTheta);
            if (float.IsNaN(direction)) direction = 0; // neutral if undefined

            // --- 5) Volatility (as erosion factor) ---
            var driftMag      = DebugOverlay.SafeFloat(vb.DriftMagnitude);
            var expArg        = DebugOverlay.SafeFloat(baseline * driftMag);
            var expVolatility = DebugOverlay.SafeFloat(MathF.Exp(DebugOverlay.Clamp(expArg, -10f, 10f)));

            // Map volatility into [0, 1) erosion factor
            // Higher volatility → more erosion (less of geomAdj survives)
            // Tunable constants:
            const float erosionScale = 0.05f;   // sensitivity to volatility
            const float erosionMax   = 0.85f;   // cap erosion so some adjustment always passes
            var erosion = DebugOverlay.Clamp(expVolatility * erosionScale, 0f, erosionMax);

            var adjusted = geomAdj * (1f - erosion) * direction;

            // --- 6) New persistence value (summary-owned) ---
            var persistence = DebugOverlay.Clamp(baseline + adjusted, -100f, 100f);

            // --- 7) Log pressure from new persistence ---
            var logArg      = 1 + persistence;
            var logPressure = (logArg <= 0f) ? 0f : DebugOverlay.SafeFloat(MathF.Log(logArg));

            // --- 8) Attach PersistenceSummary ---
            var summary = new PersistenceSummary
            {
                Current         = persistence,
                LogPressure     = logPressure,
                ExpVolatility   = expVolatility,
                Direction       = direction,
                GeometricAdj    = geomAdj,
                ErosionFactor   = erosion,
                AdjustedDelta   = adjusted,
            };
            vb.AddSummary(summary);

            // Debug / minimal output
            Console.WriteLine($"[PersistenceResolver] summary attached for Tick {vb.TickId}");

            return new ResolutionResult(vb);
        }
    }
}