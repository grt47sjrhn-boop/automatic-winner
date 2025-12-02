using System;
using System.Linq;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Resolves intent actions based on tone, persistence, volatility, area, and trait duality.
    /// Produces updated VectorBias plus a DeltaSummary for persistence/narration.
    /// </summary>
    public class IntentActionResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            vb.Intent = IntentType.None;

            if (vb.ToneTuple.Equals(default(ToneTuple)))
                return new ResolutionResult(vb, default);

            bool hasDuality = vb.Traits?.Any(t => t.Tags.Contains(TraitTag.Duality)) ?? false;

            float persistence = DebugOverlay.SafeFloat(vb.Persistence);
            float volatility  = DebugOverlay.SafeFloat(vb.ExpVolatility);
            float area        = DebugOverlay.SafeFloat(vb.Area);

            if (vb.ToneTuple.Primary == Tone.Resonance && persistence > 2.5f)
                vb.Intent = IntentType.Stabilize;
            else if (vb.ToneTuple.Primary == Tone.Scar && volatility > 1.2f)
                vb.Intent = IntentType.Disrupt;
            else if (vb.ToneTuple.Primary == Tone.Neutral && hasDuality)
                vb.Intent = IntentType.Reflect;
            else if (vb.ToneTuple.Primary == Tone.Harmony && area > 10f)
                vb.Intent = IntentType.Amplify;
            // NEW: Creation, Destruction, Transformation
            else if (vb.ToneTuple.Primary == Tone.Equilibrium && persistence > 5f && volatility < 1f)
                vb.Intent = IntentType.Creation;   // strong stability births new state
            else if (vb.ToneTuple.Primary == Tone.Fracture && persistence < -5f && volatility > 2f)
                vb.Intent = IntentType.Destruction; // deep fracture collapses bias
            else if (vb.ToneTuple.Primary == Tone.Resonance && volatility > 3f)
                vb.Intent = IntentType.Transformation; // resonance under high volatility mutates into new form


            Console.WriteLine(
                $"[IntentActionResolver] Tone={vb.ToneTuple.Primary}, " +
                $"Persistence={persistence:F2}, Volatility={volatility:F2}, Area={area:F2}, " +
                $"Duality={hasDuality}, Intent={vb.Intent}"
            );

            // Build a DeltaSummary snapshot (even if not geometric, keeps consistency)
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