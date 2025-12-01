using System.Collections.Generic;
using substrate_core.Utilities;
using substrate_shared.enums;
using substrate_shared.interfaces;
using substrate_shared.types.models;
using substrate_shared.types.structs;

namespace substrate_core.Resolvers
{
    public class ToneClusterResolver : IResolver
    {
        public VectorBias Resolve(VectorBias vb, Mood mv)
        {
            if (DebugOverlay.SafeFloat(vb.SinTheta) == 0f &&
                DebugOverlay.SafeFloat(vb.CosTheta) == 0f &&
                DebugOverlay.SafeFloat(vb.TanTheta) == 0f)
                return vb;

            Tone primary = ResolvePrimary(vb.MoodAxis);
            Tone adj1 = ResolvePrimary(vb.MoodAxis - 2);
            Tone adj2 = ResolvePrimary(vb.MoodAxis + 2);
            Tone complementary = ResolvePrimary(-vb.MoodAxis);

            var undertones = new List<Tone>();
            if (DebugOverlay.SafeFloat(vb.SinTheta) > 0.7f) undertones.Add(Tone.Irony);
            if (DebugOverlay.SafeFloat(vb.TanTheta) > 1.2f) undertones.Add(Tone.Volatility);

            vb.ToneTuple = new ToneTuple
            {
                Primary = primary,
                Adjacent1 = adj1,
                Adjacent2 = adj2,
                Complementary = complementary
            };

            var cluster = new List<Tone>
            {
                vb.ToneTuple.Primary,
                vb.ToneTuple.Adjacent1,
                vb.ToneTuple.Adjacent2,
                vb.ToneTuple.Complementary
            };
            cluster.AddRange(undertones);

            vb.ToneCluster = cluster;

            DebugOverlay.LogResolver(nameof(ToneClusterResolver), vb);

            return vb;
        }

        private Tone ResolvePrimary(int axis) => axis switch
        {
            <= -8 => Tone.Fracture,
            <= -3 => Tone.Scar,
            <= 2  => Tone.Neutral,
            <= 7  => Tone.Resonance,
            _     => Tone.Harmony
        };
    }
}