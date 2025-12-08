using System.Collections.Generic;
using substrate_shared.Facets.Enums;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.Mappers
{
    public static class FacetToneMapper
    {
        /// <summary>
        /// Convert facet distribution into a tone distribution for forging.
        /// </summary>
        public static IReadOnlyDictionary<ToneType,int> ToToneDictionary(FacetDistribution facets)
        {
            var tones = new Dictionary<ToneType,int>();

            // Example mapping logic — adjust to your design
            tones[ToneType.Resilient] = facets.Values[FacetType.Resilience];
            tones[ToneType.Harmonious] = facets.Values[FacetType.Harmony];
            tones[ToneType.Conflict]   = facets.Values[FacetType.Conflict];
            tones[ToneType.Radiant]    = facets.Values[FacetType.Radiance];

            return tones;
        }
    }
}