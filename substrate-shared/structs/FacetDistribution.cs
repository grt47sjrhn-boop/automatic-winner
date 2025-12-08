using System.Collections.Generic;
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
    }
}