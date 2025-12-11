using substrate_shared.Enums;
using substrate_shared.Profiles;
using substrate_shared.structs;

namespace substrate_shared.interfaces.Managers
{
    public interface IBiasManager : IManager
    {
        BiasDescriptor Summarize(FacetDistribution shape);
        BiasDescriptor Combine(FacetDistribution shape, BiasDescriptor seedBias);
        BiasVector Resolve(BiasVector duelistA, BiasVector duelistB, BiasDescriptor bias);

        // Simple opponent generator
        BiasVector GenerateOpponent();

        // Tuned opponent generator with profile, environment, and optional tilt
        BiasVector GenerateOpponentWeighted(IOpponentProfile profile, EnvironmentMood mood, BiasDescriptor? tilt = null);
    }
}