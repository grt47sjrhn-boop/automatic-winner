using System;
using substrate_core.Utilities;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.enums.Extensions;
using substrate_shared.types.structs;
using substrate_shared.types.Summaries;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Resolves the geometric delta between the current VectorBias and an incoming Mood stimulus.
    /// Produces a DeltaSummary (hypotenuse, area, angle, and trigonometric components)
    /// for downstream persistence/volatility calculations.
    /// </summary>
    public class DeltaVectorResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood incoming)
        {
            // Axis difference between incoming mood and current bias
            var deltaAxis = DebugOverlay.SafeFloat(incoming.MoodAxis - vb.MoodAxis);

            // Derive magnitude contextually (axis + volatility difference)
            var magnitude = DebugOverlay.SafeFloat(incoming.MagnitudeFrom(vb.CurrentMood));

            // Geometric values
            var hypotenuse = DebugOverlay.SafeFloat(MathF.Sqrt(MathF.Pow(deltaAxis, 2) + MathF.Pow(magnitude, 2)));
            var area       = DebugOverlay.SafeFloat(deltaAxis * magnitude);
            var angleTheta = DebugOverlay.SafeFloat(MathF.Atan2(magnitude, deltaAxis));

            // Trigonometric breakdown
            var sinTheta = DebugOverlay.SafeFloat(MathF.Sin(angleTheta));
            var cosTheta = DebugOverlay.SafeFloat(MathF.Cos(angleTheta));
            var tanTheta = DebugOverlay.SafeFloat(DebugOverlay.Clamp(MathF.Tan(angleTheta), -100f, 100f));

            // Build summary
            var summary = new DeltaSummary
            {
                DeltaAxis   = deltaAxis,
                Magnitude   = magnitude,
                Hypotenuse  = hypotenuse,
                Area        = area,
                AngleTheta  = angleTheta,
                SinTheta    = sinTheta,
                CosTheta    = cosTheta,
                TanTheta    = tanTheta
            };

            // Attach summary to bias (add, not overwrite)
            vb.AddSummary(summary);

            // Debug / minimal output
            Console.WriteLine($"[DeltaVectorResolver] summary attached for Tick {vb.TickId}");

            // Return updated bias wrapped in ResolutionResult
            return new ResolutionResult(vb);
        }
    }
}