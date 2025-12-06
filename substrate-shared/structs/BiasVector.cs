using System;
using substrate_shared.Registries.Base;

namespace substrate_shared.structs
{
    public readonly struct BiasVector(NarrativeTone tone, int magnitude)
    {
        public NarrativeTone Tone { get; } = tone ?? throw new ArgumentNullException(nameof(tone));
        public int Magnitude { get; } = magnitude;

        public int SignedStrength => Tone.BiasMultiplier * Magnitude; // BiasValue ∈ {-1,0,+1}

        public override string ToString() =>
            $"{Tone.Label} (Bias: {Tone.BiasValue}, Magnitude: {Magnitude})";
    }
}