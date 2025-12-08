using System;
using System.Collections.Generic;
using substrate_shared.Facets.Enums;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.Registries.Extensions;

namespace substrate_shared.structs;

public readonly struct BiasVector
{
    public NarrativeTone Tone { get; }
    public int Magnitude { get; }

    public BiasVector(NarrativeTone tone, int magnitude)
    {
        Tone = tone ?? throw new ArgumentNullException(nameof(tone));
        Magnitude = magnitude;
    }

    public int SignedStrength => Tone.BiasMultiplier * Magnitude;

    /// <summary>
    /// The dominant tone type represented by this vector.
    /// </summary>
    public ToneType DominantTone => Tone.Type;

    public override string ToString() =>
        $"{Tone.Label} (Bias: {Tone.BiasValue}, Magnitude: {Magnitude})";

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

        int magnitude = rng.Next(1, 11);
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
        foreach (FacetType type in (FacetType[])Enum.GetValues(typeof(FacetType)))
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
                // Neutral or other tones can be mapped to Harmony by default
                values[FacetType.Harmony] = Magnitude;
                break;
        }

        return new FacetDistribution(values);
    }

}