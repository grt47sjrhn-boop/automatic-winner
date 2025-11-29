using System;
using System.Runtime.CompilerServices;
using substrate_shared.structs;

namespace substrate_shared.types.core
{
    /// <summary>
    /// Immutable input vector representing mood axes.
    /// Provides conversion to BiasDelta and magnitude calculation.
    /// </summary>
    public readonly struct MoodVector
    {
        public float Resonance { get; }
        public float Warmth { get; }
        public float Irony { get; }
        public float Flux { get; }
        public float Inertia { get; }
        public float Variance { get; }
        public float HiddenPressure { get; }

        public MoodVector(
            float resonance,
            float warmth,
            float irony,
            float flux,
            float inertia,
            float variance,
            float hiddenPressure)
        {
            Resonance = resonance;
            Warmth = warmth;
            Irony = irony;
            Flux = flux;
            Inertia = inertia;
            Variance = variance;
            HiddenPressure = hiddenPressure;
        }

        /// <summary>
        /// Convert to a BiasDelta relative to the current bias snapshot.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BiasDelta ToBiasDelta(in BiasVector bias)
        {
            return new BiasDelta(
                Resonance - bias.Resonance,
                Warmth - bias.Warmth,
                Irony - bias.Irony,
                Flux - bias.Flux,
                Inertia - bias.Inertia,
                Variance - bias.Variance,
                HiddenPressure - bias.HiddenPressure
            );
        }

        /// <summary>
        /// L1 magnitude (sum of absolute values).
        /// Fast and consistent with resolution lens thresholds.
        /// </summary>
        public float MagnitudeL1 =>
            MathF.Abs(Resonance) + MathF.Abs(Warmth) + MathF.Abs(Irony) +
            MathF.Abs(Flux) + MathF.Abs(Inertia) + MathF.Abs(Variance) +
            MathF.Abs(HiddenPressure);

        /// <summary>
        /// L2 magnitude (Euclidean norm).
        /// Use if you want smoother scaling.
        /// </summary>
        public float MagnitudeL2
        {
            get
            {
                float sum = Resonance * Resonance + Warmth * Warmth + Irony * Irony +
                            Flux * Flux + Inertia * Inertia + Variance * Variance +
                            HiddenPressure * HiddenPressure;
                return MathF.Sqrt(sum);
            }
        }
    }
}