using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Registries.Base;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_core.Managers
{
    /// <summary>
    /// Manager responsible for summarizing and resolving bias vectors.
    /// Implements IManager for orchestration consistency.
    /// </summary>
    public class BiasManager : IManager
    {
        /// <summary>
        /// Summarize a facet distribution into a BiasDescriptor.
        /// </summary>
        public BiasDescriptor Summarize(FacetDistribution shape)
        {
            int resilience = shape.Values[FacetType.Resilience];
            int conflict   = shape.Values[FacetType.Conflict];
            int harmony    = shape.Values[FacetType.Harmony];
            int radiance   = shape.Values[FacetType.Radiance];

            Bias bias;
            if (resilience > conflict) bias = Bias.Positive;
            else if (conflict > resilience) bias = Bias.Negative;
            else if (harmony > 0) bias = Bias.Neutral;
            else bias = Bias.Mixed;

            string notes = $"Resilience({resilience}), Conflict({conflict}), Harmony({harmony}), Radiance({radiance})";

            return new BiasDescriptor
            {
                Bias = bias,
                Narrative = $"Bias summary → {notes}"
            };
        }

        /// <summary>
        /// Combine a facet distribution with a seed bias descriptor.
        /// </summary>
        public BiasDescriptor Combine(FacetDistribution shape, BiasDescriptor seedBias)
        {
            var combined = Summarize(shape);

            // Tilt toward seed bias if provided
            if (seedBias.Bias != Bias.Neutral)
                combined.Bias = seedBias.Bias;

            combined.Narrative += $" | Seed bias tilt → {seedBias.Bias}";
            return combined;
        }

        /// <summary>
        /// Resolve two bias vectors into one, optionally tilting by a seed bias.
        /// </summary>
        public BiasVector Resolve(BiasVector a, BiasVector b, BiasDescriptor? seedBias = null)
        {
            var resolved = a.SignedStrength >= b.SignedStrength ? a : b;

            if (seedBias == null || seedBias.Bias == Bias.Neutral) return resolved;
            var tiltedTone = new NarrativeTone(
                resolved.Tone.Type,
                resolved.Tone.Label,
                resolved.Tone.Category,
                seedBias.Bias,
                resolved.Tone.Group
            );

            resolved = new BiasVector(tiltedTone, resolved.Magnitude);

            return resolved;
        }
    }
}