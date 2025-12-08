using System.Collections.Generic;
using System.Linq;
using substrate_shared.Facets.Enums;
using substrate_shared.interfaces;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_core.Managers
{
    /// <summary>
    /// Manager responsible for interpreting facet distributions into bias descriptors.
    /// Implements IManager for orchestration consistency.
    /// </summary>
    public class FacetManager : IManager
    {
        /// <summary>
        /// Translate facet values into a BiasDescriptor with narrative notes.
        /// </summary>
        public BiasDescriptor Narrate(FacetDistribution shape)
        {
            int resilience = shape.Values[FacetType.Resilience];
            int harmony    = shape.Values[FacetType.Harmony];
            int conflict   = shape.Values[FacetType.Conflict];
            int radiance   = shape.Values[FacetType.Radiance];

            // Bias logic
            Bias bias;
            if (resilience > conflict) bias = Bias.Positive;
            else if (conflict > resilience) bias = Bias.Negative;
            else if (harmony > 0) bias = Bias.Neutral;
            else bias = Bias.Mixed;

            // Narrative notes
            string notes = $"Resilience({resilience}), Harmony({harmony}), Conflict({conflict}), Radiance({radiance})";

            return new BiasDescriptor
            {
                Bias = bias,
                Narrative = $"Facet distribution → {notes}"
            };
        }

        /// <summary>
        /// Normalize raw facet values into a FacetDistribution.
        /// Ensures all expected facet types are present and scales values proportionally.
        /// </summary>
        public static FacetDistribution Normalize(IDictionary<FacetType,int> rawValues)
        {
            // Ensure all facet types exist in the dictionary
            var normalized = new Dictionary<FacetType,int>();
            foreach (FacetType type in (FacetType[])System.Enum.GetValues(typeof(FacetType)))
            {
                normalized[type] = rawValues.TryGetValue(type, out var value) ? value : 0;
            }

            // Scale values so the highest facet = 10 (or leave as-is if all zero)
            int max = normalized.Values.Max();
            if (max > 0)
            {
                foreach (var key in normalized.Keys.ToList())
                {
                    normalized[key] = (int)System.Math.Round((normalized[key] / (double)max) * 10);
                }
            }

            return new FacetDistribution(normalized);
        }

        public static FacetDistribution Aggregate(IEnumerable<FacetDistribution> distributions)
        {
            if (distributions == null || !distributions.Any())
            {
                // Return an empty normalized distribution
                return Normalize(new Dictionary<FacetType,int>());
            }

            var aggregate = new Dictionary<FacetType,int>();

            // Initialize all facet types to 0
            foreach (FacetType type in (FacetType[])System.Enum.GetValues(typeof(FacetType)))
            {
                aggregate[type] = 0;
            }

            // Sum values across all distributions
            foreach (var dist in distributions)
            {
                foreach (var kv in dist.Values)
                {
                    aggregate[kv.Key] += kv.Value;
                }
            }

            // Normalize the aggregate so values are scaled consistently
            return Normalize(aggregate);
        }
    }
}