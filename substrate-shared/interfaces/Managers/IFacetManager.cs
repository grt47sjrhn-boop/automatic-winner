using System.Collections.Generic;
using substrate_shared.Enums;
using substrate_shared.Registries.enums;
using substrate_shared.structs;

namespace substrate_shared.interfaces.Managers
{
    public interface IFacetManager : IManager
    {
        FacetDistribution Normalize(IReadOnlyDictionary<FacetType,int> values);
        FacetDistribution Aggregate(IEnumerable<FacetDistribution> distributions);
        IReadOnlyDictionary<ToneType, int> ResolveFacets(BiasVector persistentBiasVector, BiasVector opponent);
    }
}