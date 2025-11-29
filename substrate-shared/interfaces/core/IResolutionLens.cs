using substrate_shared.structs;
using substrate_shared.types.core;
using ResolutionLayers = substrate_shared.structs.ResolutionLayers;

namespace substrate_shared.interfaces.core
{
    public interface IResolutionLens
    {
        ResolutionLayers Resolve(in BiasDelta delta, in BiasVector bias, float magnitude);
    }
}