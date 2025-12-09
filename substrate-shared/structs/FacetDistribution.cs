using System.Collections.Generic;
using System.Linq;
using substrate_shared.Facets.Enums;

namespace substrate_shared.structs
{
    public class FacetDistribution
    {
        public Dictionary<FacetType,int> Values { get; } = new();

        public FacetDistribution() { }

        public FacetDistribution(Dictionary<FacetType,int> values)
        {
            Values = values ?? new Dictionary<FacetType,int>();
        }

        /// <summary>
        /// Computed threshold: the maximum facet value in the distribution.
        /// Returns 0 if no facets are present.
        /// </summary>
        public int Threshold => Values.Count == 0 ? 0 : Values.Values.Max();
    }
}