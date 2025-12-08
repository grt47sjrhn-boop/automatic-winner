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
    /// Implements IFacetManager for orchestration consistency.
    /// </summary>
    public class FacetManager : IFacetManager
    {
        /// <summary>
        /// Translate facet values into a BiasDescriptor with narrative notes.
        /// </summary>
        public BiasDescriptor Narrate(FacetDistribution shape)
        {
            var resilience = shape.Values[FacetType.Resilience];
            var harmony    = shape.Values[FacetType.Harmony];
            var conflict   = shape.Values[FacetType.Conflict];
            var radiance   = shape.Values[FacetType.Radiance];

            Bias bias;
            if (resilience > conflict) bias = Bias.Positive;
            else if (conflict > resilience) bias = Bias.Negative;
            else if (harmony > 0) bias = Bias.Neutral;
            else bias = Bias.Mixed;

            var notes = $"Resilience({resilience}), Harmony({harmony}), Conflict({conflict}), Radiance({radiance})";

            return new BiasDescriptor
            {
                Bias = bias,
                Narrative = $"Facet distribution → {notes}"
            };
        }

        /// <summary>
        /// Static helper: normalize raw facet values into a FacetDistribution.
        /// </summary>
        private static FacetDistribution NormalizeInternal(IDictionary<FacetType,int> rawValues)
        {
            var normalized = new Dictionary<FacetType,int>();
            foreach (var type in (FacetType[])System.Enum.GetValues(typeof(FacetType)))
            {
                normalized[type] = rawValues.TryGetValue(type, out var value) ? value : 0;
            }

            var max = normalized.Values.Max();
            if (max > 0)
            {
                foreach (var key in normalized.Keys.ToList())
                {
                    normalized[key] = (int)System.Math.Round((normalized[key] / (double)max) * 10);
                }
            }

            return new FacetDistribution(normalized);
        }

        /// <summary>
        /// Instance implementation of Normalize for IFacetManager.
        /// </summary>
        public FacetDistribution Normalize(IReadOnlyDictionary<FacetType, int> values)
        {
            // Convert to mutable dictionary and delegate to internal helper
            return NormalizeInternal(values.ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        /// <summary>
        /// Aggregate multiple facet distributions into one normalized distribution.
        /// </summary>
        public FacetDistribution Aggregate(IEnumerable<FacetDistribution> distributions)
        {
            if (distributions == null || !distributions.Any())
            {
                return NormalizeInternal(new Dictionary<FacetType,int>());
            }

            var aggregate = new Dictionary<FacetType,int>();
            foreach (var type in (FacetType[])System.Enum.GetValues(typeof(FacetType)))
            {
                aggregate[type] = 0;
            }

            foreach (var dist in distributions)
            {
                foreach (var kv in dist.Values)
                {
                    aggregate[kv.Key] += kv.Value;
                }
            }

            return NormalizeInternal(aggregate);
        }
    }
}