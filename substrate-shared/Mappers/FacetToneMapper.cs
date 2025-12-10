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
        public static Dictionary<ToneType, int> ToToneDictionary(FacetDistribution facets)
        {
            var dict = new Dictionary<ToneType, int>();

            dict[ToneType.Resilient] = facets.Values.TryGetValue(FacetType.Resilience, out var res) ? res : 0;
            dict[ToneType.Harmonious] = facets.Values.TryGetValue(FacetType.Harmony, out var harm) ? harm : 0;
            dict[ToneType.Conflict] = facets.Values.TryGetValue(FacetType.Conflict, out var conf) ? conf : 0;
            dict[ToneType.Radiant] = facets.Values.TryGetValue(FacetType.Radiance, out var rad) ? rad : 0;

            return dict;
        }

    }
}