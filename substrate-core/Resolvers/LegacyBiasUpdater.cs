using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Updates the VectorBias.Legacy field based on the resolved tone cluster.
    /// Ensures crystallization/fragmentation events narrate with ResilientHarmony, FracturedLegacy, or Equilibrium.
    /// </summary>
    internal class LegacyBiasUpdater : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            vb.Legacy = vb.ToneTuple.Primary switch
            {
                Tone.Harmony   => LegacyTraitLock.ResilientHarmony,
                Tone.Resonance => LegacyTraitLock.ResilientHarmony,
                Tone.Scar      => LegacyTraitLock.FracturedLegacy,
                Tone.Fracture  => LegacyTraitLock.FracturedLegacy,
                Tone.Neutral   => LegacyTraitLock.Equilibrium, // NEW mapping
                _              => LegacyTraitLock.None
            };

            DebugOverlay.LogResolver(nameof(LegacyBiasUpdater), vb);

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