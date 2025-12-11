using substrate_shared.interfaces.Details;
using substrate_shared.structs;

namespace substrate_shared.interfaces
{
    public interface IRarityManager : IManager
    {
        IRarityTier AssignTier(int score);
        // NEW overload
        IRarityTier AssignTier(IEngagement engagement, double hypotenuse, double area, double cos, double sin, double avgHypotenuse, double avgArea);

        // 🔹 New overload for facet distributions
        int ComputeScore(FacetDistribution distribution);
        
        int ComputeScore(IEngagement engagement);
    }
}