using System.Collections.Generic;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    /// <summary>
    /// Resolves tone clusters based on the incoming mood stimulus axis and trigonometric values.
    /// Produces updated VectorBias plus a DeltaSummary snapshot for persistence/narration.
    /// </summary>
    public class ToneClusterResolver : IResolver
    {
        public ResolutionResult Resolve(VectorBias vb, Mood mv)
        {
            if (DebugOverlay.SafeFloat(vb.SinTheta) == 0f &&
                DebugOverlay.SafeFloat(vb.CosTheta) == 0f &&
                DebugOverlay.SafeFloat(vb.TanTheta) == 0f)
            {
                return new ResolutionResult(vb, default);
            }

            // Use incoming mood axis for tone resolution
            int axis = (int)mv.MoodAxis;

            Tone primary       = ResolvePrimary(axis);
            Tone adj1          = ResolvePrimary(axis - 2);
            Tone adj2          = ResolvePrimary(axis + 2);
            Tone complementary = ResolvePrimary(-axis);

            // Deduplicate undertones
            var undertones = new HashSet<Tone>();
            if (DebugOverlay.SafeFloat(vb.SinTheta) > 0.7f) undertones.Add(Tone.Irony);
            if (DebugOverlay.SafeFloat(vb.TanTheta) > 1.2f) undertones.Add(Tone.Volatility);

            vb.ToneTuple = new ToneTuple
            {
                Primary       = primary,
                Adjacent1     = adj1,
                Adjacent2     = adj2,
                Complementary = complementary
            };

            var cluster = new List<Tone>
            {
                vb.ToneTuple.Primary,
                vb.ToneTuple.Adjacent1,
                vb.ToneTuple.Adjacent2,
                vb.ToneTuple.Complementary
            };
            cluster.AddRange(undertones); // no duplicates

            vb.ToneCluster = cluster;

            DebugOverlay.LogResolver(nameof(ToneClusterResolver), vb);

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

        private Tone ResolvePrimary(int axis) => axis switch
        {
            <= -9 => Tone.Fracture,
            <= -5 => Tone.Scar,
            <= -1 => Tone.Neutral,   // Neutral will later map to LegacyTraitLock.Equilibrium
            <= 4  => Tone.Resonance,
            _     => Tone.Harmony
        };
    }
}