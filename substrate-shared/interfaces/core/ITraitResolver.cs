using substrate_shared.structs;
using substrate_shared.types.core;
using ArtifactCore = substrate_shared.structs.ArtifactCore;
using ResolutionLayers = substrate_shared.structs.ResolutionLayers;

namespace substrate_shared.interfaces.core
{
    public interface ITraitResolver
    {
        Trait Resolve(in ArtifactCore core, in ResolutionLayers layers, in BiasVector bias, float magnitude);
    }
}