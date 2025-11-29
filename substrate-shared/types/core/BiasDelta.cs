using System;
using System.Runtime.CompilerServices;

namespace substrate_shared.types.core
{
    public readonly struct BiasDelta
    {
        public readonly float Resonance;
        public readonly float Warmth;
        public readonly float Irony;
        public readonly float Flux;
        public readonly float Inertia;
        public readonly float Variance;
        public readonly float HiddenPressure;

        public BiasDelta(float resonance, float warmth, float irony, float flux, float inertia, float variance, float hiddenPressure)
        {
            Resonance = resonance;
            Warmth = warmth;
            Irony = irony;
            Flux = flux;
            Inertia = inertia;
            Variance = variance;
            HiddenPressure = hiddenPressure;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Magnitude()
        {
            // Euclidean norm across axes
            double sum =
                (double)Resonance * Resonance +
                (double)Warmth * Warmth +
                (double)Irony * Irony +
                (double)Flux * Flux +
                (double)Inertia * Inertia +
                (double)Variance * Variance +
                (double)HiddenPressure * HiddenPressure;

            return (float)Math.Sqrt(sum);
        }
    }
}