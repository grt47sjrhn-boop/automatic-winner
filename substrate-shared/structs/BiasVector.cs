using System;
using substrate_shared.Registries.Base;

namespace substrate_shared.structs
{
    public readonly struct BiasVector(NarrativeTone tone, int magnitude)
    {
        public NarrativeTone Tone { get; } = tone ?? throw new ArgumentNullException(nameof(tone));
        public int Magnitude { get; } = magnitude;

        public override string ToString() =>
            $"{Tone.Label} ({Tone.BiasValue}, Magnitude: {Magnitude})";
    }
}