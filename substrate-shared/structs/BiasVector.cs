using System;
using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions;

namespace substrate_shared.structs
{
    /// <summary>
    /// Represents a directional bias vector with a narrative tone and magnitude.
    /// </summary>
    public readonly struct BiasVector
    {
        public NarrativeTone Tone { get; }
        public int Magnitude { get; }

        public BiasVector(NarrativeTone tone, int magnitude)
        {
            Tone = tone ?? throw new ArgumentNullException(nameof(tone));
            Magnitude = magnitude;
        }

        /// <summary>
        /// Signed strength of this vector, factoring in bias polarity.
        /// </summary>
        public int SignedStrength
        {
            get
            {
                // Defensive guard: if Tone is null, treat as neutral
                if (Tone == null) return 0;
                return Tone.BiasMultiplier * Magnitude;
            }
        }

        /// <summary>
        /// The dominant tone type represented by this vector.
        /// </summary>
        public ToneType DominantTone => Tone?.Type ?? ToneType.Neutral;

        public override string ToString()
        {
            if (Tone == null)
                return $"[Null Tone] (Magnitude: {Magnitude})";

            return $"{Tone.Label} (Bias: {Tone.BiasValue}, Magnitude: {Magnitude})";
        }

        /// <summary>
        /// Generate a random bias vector with a random tone and magnitude.
        /// </summary>
        public static BiasVector GenerateRandom()
        {
            var rng = new Random();
            var toneTypes = (ToneType[])Enum.GetValues(typeof(ToneType));
            var randomToneType = toneTypes[rng.Next(toneTypes.Length)];

            var tone = new NarrativeTone(
                type: randomToneType,
                label: randomToneType.GetDescription(),
                category: randomToneType.GetGroup().ToString(),
                biasValue: randomToneType.GetBias(),
                group: randomToneType.GetGroup().ToString()
            );

            var magnitude = rng.Next(1, 11);
            return new BiasVector(tone, magnitude);
        }

        /// <summary>
        /// Convert this bias vector into a FacetDistribution.
        /// Maps the dominant tone into its corresponding facet axis.
        /// </summary>
        public FacetDistribution ToFacetDistribution()
        {
            var values = new Dictionary<FacetType, int>();

            // Initialize all facet types to 0
            foreach (var type in (FacetType[])Enum.GetValues(typeof(FacetType)))
            {
                values[type] = 0;
            }

            // Map dominant tone into a facet axis
            switch (DominantTone)
            {
                case ToneType.Resilient:
                    values[FacetType.Resilience] = Magnitude;
                    break;
                case ToneType.Harmonious:
                    values[FacetType.Harmony] = Magnitude;
                    break;
                case ToneType.Conflict:
                case ToneType.Hostile:
                case ToneType.Critical:
                    values[FacetType.Conflict] = Magnitude;
                    break;
                case ToneType.Radiant:
                    values[FacetType.Radiance] = Magnitude;
                    break;
                default:
                    // Neutral or other tones map to Harmony by default
                    values[FacetType.Harmony] = Magnitude;
                    break;
            }

            return new FacetDistribution(values);
        }
    }
}