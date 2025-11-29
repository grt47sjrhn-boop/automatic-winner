using substrate_shared.structs;
using substrate_shared.types.core;
using ArtifactCore = substrate_shared.structs.ArtifactCore;
using ResolutionLayers = substrate_shared.structs.ResolutionLayers;

namespace substrate_shared.interfaces.core
{
    public interface IBiasDriftPolicy
    {
        BiasVector Apply(in BiasVector current, in BiasDelta delta, float magnitude, in ArtifactCore artifact, in ResolutionLayers layers);
    }
}