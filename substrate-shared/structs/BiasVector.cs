using System;
using System.Runtime.CompilerServices;
using substrate_shared.types.core;

namespace substrate_shared.structs
{
    /// <summary>
    /// Immutable vector representing substrate bias across multiple axes.
    /// Provides value-based equality and scaling operations.
    /// </summary>
    public readonly struct BiasVector : IEquatable<BiasVector>
    {
        public float Resonance { get; }
        public float Warmth { get; }
        public float Irony { get; }
        public float Flux { get; }
        public float Inertia { get; }
        public float Variance { get; }
        public float HiddenPressure { get; }

        public BiasVector(
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
        /// A zero bias vector (all components = 0).
        /// </summary>
        public static BiasVector Zero => new BiasVector(0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        /// Adds a scaled bias delta to this vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BiasVector AddScaled(in BiasDelta delta, float alpha)
        {
            return new BiasVector(
                Resonance + delta.Resonance * alpha,
                Warmth + delta.Warmth * alpha,
                Irony + delta.Irony * alpha,
                Flux + delta.Flux * alpha,
                Inertia + delta.Inertia * alpha,
                Variance + delta.Variance * alpha,
                HiddenPressure + delta.HiddenPressure * alpha
            );
        }

        #region Equality

        public bool Equals(BiasVector other) =>
            Resonance == other.Resonance &&
            Warmth == other.Warmth &&
            Irony == other.Irony &&
            Flux == other.Flux &&
            Inertia == other.Inertia &&
            Variance == other.Variance &&
            HiddenPressure == other.HiddenPressure;

        public override bool Equals(object? obj) =>
            obj is BiasVector other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(Resonance, Warmth, Irony, Flux, Inertia, Variance, HiddenPressure);

        public static bool operator ==(BiasVector left, BiasVector right) => left.Equals(right);
        public static bool operator !=(BiasVector left, BiasVector right) => !left.Equals(right);

        #endregion

        public override string ToString() =>
            $"BiasVector(R={Resonance}, W={Warmth}, I={Irony}, F={Flux}, In={Inertia}, V={Variance}, H={HiddenPressure})";
    }
}