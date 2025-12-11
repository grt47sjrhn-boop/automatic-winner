using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.interfaces;
using substrate_shared.interfaces.Managers;
using substrate_shared.Mappers;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_core.Managers
{
    public class FacetManager : IFacetManager
    {
        public FacetDistribution Normalize(IReadOnlyDictionary<FacetType,int> values)
        {
            var distribution = new FacetDistribution();

            if (values == null) return distribution;

            // Simple normalization: scale values to 0–100 range
            var max = 0;
            foreach (var kv in values)
                if (kv.Value > max) max = kv.Value;

            foreach (var kv in values)
                distribution.Values[kv.Key] = max > 0 ? (int)((kv.Value / (double)max) * 100) : 0;

            return distribution;
        }

        public FacetDistribution Aggregate(IEnumerable<FacetDistribution> distributions)
        {
            var result = new FacetDistribution();

            foreach (var dist in distributions)
            {
                foreach (var kv in dist.Values)
                {
                    if (result.Values.ContainsKey(kv.Key))
                        result.Values[kv.Key] += kv.Value;
                    else
                        result.Values[kv.Key] = kv.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Resolve facets from two bias vectors into a tone dictionary for forging.
        /// </summary>
        public IReadOnlyDictionary<ToneType,int> ResolveFacets(BiasVector persistentBiasVector, BiasVector opponent)
        {
            var combined = new Dictionary<FacetType,int>();

            void AddFacets(BiasVector vector)
            {
                var dist = vector.ToFacetDistribution();
                foreach (var kv in dist.Values)
                {
                    if (combined.ContainsKey(kv.Key))
                        combined[kv.Key] += kv.Value;
                    else
                        combined[kv.Key] = kv.Value;
                }
            }
            
            AddFacets(persistentBiasVector);
            AddFacets(opponent);

            // Normalize combined facets
            var normalized = Normalize(combined);

            // 🔹 Map normalized facets into tone dictionary
            return FacetToneMapper.ToToneDictionary(normalized);
        }
    }
}