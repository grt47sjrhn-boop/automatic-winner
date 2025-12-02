using System;
using substrate_core.Utilities;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;
using substrate_shared.enums;
using substrate_shared.enums.Extensions;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Resolves the geometric delta between the current VectorBias and an incoming Mood stimulus.
    /// Produces hypotenuse, area, angle, and trigonometric components for downstream persistence/volatility calculations.
    /// </summary>
    public class DeltaVectorResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood incoming)
        {
            // Axis difference between incoming mood and current bias
            float deltaAxis = DebugOverlay.SafeFloat(incoming.MoodAxis - vb.MoodAxis);

            // Derive magnitude contextually (axis + volatility difference)
            float magnitude = DebugOverlay.SafeFloat(incoming.MagnitudeFrom(vb.CurrentMood));

            // Geometric values
            vb.Hypotenuse = DebugOverlay.SafeFloat(MathF.Sqrt(MathF.Pow(deltaAxis, 2) + MathF.Pow(magnitude, 2)));
            vb.Area       = DebugOverlay.SafeFloat(deltaAxis * magnitude);
            vb.AngleTheta = DebugOverlay.SafeFloat(MathF.Atan2(magnitude, deltaAxis));

            // Trigonometric breakdown
            vb.SinTheta = DebugOverlay.SafeFloat(MathF.Sin(vb.AngleTheta));
            vb.CosTheta = DebugOverlay.SafeFloat(MathF.Cos(vb.AngleTheta));
            vb.TanTheta = DebugOverlay.SafeFloat(DebugOverlay.Clamp(MathF.Tan(vb.AngleTheta), -100f, 100f));

            // Build summary for persistence
            var summary = new DeltaSummary
            {
                DeltaAxis   = deltaAxis,
                Magnitude   = magnitude,
                Hypotenuse  = vb.Hypotenuse,
                Area        = vb.Area,
                AngleTheta  = vb.AngleTheta,
                SinTheta    = vb.SinTheta,
                CosTheta    = vb.CosTheta,
                TanTheta    = vb.TanTheta
            };

            // Debug / narratable output
            Console.WriteLine(
                $"[DeltaVectorResolver] ΔAxis={deltaAxis:F2}, Mag={magnitude:F2}, " +
                $"Hyp={vb.Hypotenuse:F2}, Area={vb.Area:F2}, Angle={vb.AngleTheta:F2}, " +
                $"Sin={vb.SinTheta:F2}, Cos={vb.CosTheta:F2}, Tan={vb.TanTheta:F2} | " +
                $"Incoming={incoming.MoodType} ({incoming.MoodType.GetNarrativeGroup()})"
            );

            return new ResolutionResult(vb, summary);
        }
    }
}