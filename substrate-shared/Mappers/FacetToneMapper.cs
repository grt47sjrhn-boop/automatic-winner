using System.Collections.Generic;
using substrate_shared.Facets.Enums;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.Mappers
{
    /// <summary>
    /// Maps facet distributions into tone distributions for forging crystals and tone cuts.
    /// </summary>
    public static class FacetToneMapper
    {
        /// <summary>
        /// Convert facet distribution into a tone distribution.
        /// </summary>
        public static IReadOnlyDictionary<ToneType,int> ToToneDictionary(FacetDistribution facets)
        {
            var dict = new Dictionary<ToneType,int>();

            if (facets?.Values == null)
                return dict;

            dict[ToneType.Resilient]  = facets.Values.GetValueOrDefault(FacetType.Resilience, 0);
            dict[ToneType.Harmonious] = facets.Values.GetValueOrDefault(FacetType.Harmony, 0);
            dict[ToneType.Conflict]   = facets.Values.GetValueOrDefault(FacetType.Conflict, 0);
            dict[ToneType.Radiant]    = facets.Values.GetValueOrDefault(FacetType.Radiance, 0);

            return dict;
        }
    }
}